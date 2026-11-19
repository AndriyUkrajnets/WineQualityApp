from pathlib import Path
from typing import Any

import joblib
import pandas as pd

from app.schemas import WineFeatures


FEATURE_COLUMNS = [
    "fixed_acidity",
    "volatile_acidity",
    "citric_acid",
    "residual_sugar",
    "chlorides",
    "free_sulfur_dioxide",
    "total_sulfur_dioxide",
    "density",
    "pH",
    "sulphates",
    "alcohol",
    "color",
]

DEFAULT_MODEL_PATH = (
    Path(__file__).resolve().parents[1]
    / "artifacts"
    / "wine_quality_random_forest.joblib"
)


def load_model(model_path: Path = DEFAULT_MODEL_PATH) -> Any:
    """Load the trusted fitted scikit-learn pipeline once during app startup."""
    if not model_path.is_file():
        raise FileNotFoundError(f"Model artifact not found: {model_path}")

    model = joblib.load(model_path)
    if not callable(getattr(model, "predict", None)):
        raise TypeError(f"Loaded object does not provide predict(): {model_path}")

    input_features = getattr(model, "feature_names_in_", None)
    if input_features is not None and list(input_features) != FEATURE_COLUMNS:
        raise ValueError(
            "Model input feature order does not match the service contract: "
            f"{list(input_features)}"
        )

    return model


def predict_quality(model: Any, wine: WineFeatures) -> float:
    """Run prediction while preserving named columns expected by the pipeline."""
    row = wine.model_dump()
    frame = pd.DataFrame([[row[column] for column in FEATURE_COLUMNS]], columns=FEATURE_COLUMNS)
    prediction = model.predict(frame)
    return float(prediction[0])
