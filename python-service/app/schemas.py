from typing import Literal

from pydantic import BaseModel, ConfigDict, Field


class WineFeatures(BaseModel):
    """The 11 physicochemical measurements and wine color used by the model."""

    model_config = ConfigDict(extra="forbid")

    fixed_acidity: float = Field(allow_inf_nan=False)
    volatile_acidity: float = Field(allow_inf_nan=False)
    citric_acid: float = Field(allow_inf_nan=False)
    residual_sugar: float = Field(allow_inf_nan=False)
    chlorides: float = Field(allow_inf_nan=False)
    free_sulfur_dioxide: float = Field(allow_inf_nan=False)
    total_sulfur_dioxide: float = Field(allow_inf_nan=False)
    density: float = Field(allow_inf_nan=False)
    pH: float = Field(allow_inf_nan=False)
    sulphates: float = Field(allow_inf_nan=False)
    alcohol: float = Field(allow_inf_nan=False)
    color: Literal["red", "white"]


class PredictionResponse(BaseModel):
    predicted_quality: float


class HealthResponse(BaseModel):
    status: Literal["ready", "model_unavailable"]
    model_ready: bool
