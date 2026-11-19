from pathlib import Path
from typing import Any

import pandas as pd
import pytest

from app.predictor import DEFAULT_MODEL_PATH, FEATURE_COLUMNS, load_model, predict_quality
from app.schemas import WineFeatures


class RecordingModel:
    def __init__(self) -> None:
        self.received: pd.DataFrame | None = None

    def predict(self, frame: pd.DataFrame) -> list[float]:
        self.received = frame.copy()
        return [6.25]


def test_predict_quality_passes_named_features_in_model_order(
    example_wine: dict[str, float | str],
) -> None:
    model = RecordingModel()
    wine = WineFeatures.model_validate(example_wine)

    result = predict_quality(model, wine)

    assert result == 6.25
    assert model.received is not None
    assert list(model.received.columns) == FEATURE_COLUMNS
    assert model.received.iloc[0].to_dict() == example_wine


def test_load_model_fails_clearly_when_artifact_is_missing(tmp_path: Path) -> None:
    with pytest.raises(FileNotFoundError, match="Model artifact not found"):
        load_model(tmp_path / "missing.joblib")


@pytest.mark.skipif(not DEFAULT_MODEL_PATH.is_file(), reason="Model artifact is not present")
def test_load_model_returns_fitted_pipeline() -> None:
    model: Any = load_model()

    assert callable(model.predict)
    assert list(model.feature_names_in_) == FEATURE_COLUMNS
    assert "preprocess" in model.named_steps
    assert "regressor" in model.named_steps
