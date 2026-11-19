import logging
import os
from contextlib import asynccontextmanager
from pathlib import Path
from typing import AsyncIterator

from fastapi import FastAPI, HTTPException, Request, status

from app.predictor import DEFAULT_MODEL_PATH, load_model, predict_quality
from app.schemas import HealthResponse, PredictionResponse, WineFeatures


logger = logging.getLogger(__name__)


@asynccontextmanager
async def lifespan(app: FastAPI) -> AsyncIterator[None]:
    """Load the model once when the service starts."""
    configured_path = os.getenv("WINE_MODEL_PATH")
    model_path = Path(configured_path) if configured_path else DEFAULT_MODEL_PATH

    try:
        app.state.model = load_model(model_path)
        logger.info("Wine quality model loaded from %s", model_path)
    except FileNotFoundError:
        app.state.model = None
        logger.warning("Wine quality model is not available at %s", model_path)

    yield


app = FastAPI(title="Wine Quality ML Service", lifespan=lifespan)


@app.get("/health", response_model=HealthResponse)
async def health(request: Request) -> HealthResponse:
    model_ready = request.app.state.model is not None
    return HealthResponse(
        status="ready" if model_ready else "model_unavailable",
        model_ready=model_ready,
    )


@app.post("/predict", response_model=PredictionResponse)
async def predict(wine: WineFeatures, request: Request) -> PredictionResponse:
    model = request.app.state.model
    if model is None:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="The wine quality model is not available.",
        )

    return PredictionResponse(predicted_quality=predict_quality(model, wine))
