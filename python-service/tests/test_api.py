from fastapi.testclient import TestClient
import pytest

from app.main import app
from app.predictor import DEFAULT_MODEL_PATH


@pytest.mark.skipif(not DEFAULT_MODEL_PATH.is_file(), reason="Model artifact is not present")
def test_health_reports_loaded_model(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.delenv("WINE_MODEL_PATH", raising=False)

    with TestClient(app) as client:
        response = client.get("/health")

    assert response.status_code == 200
    assert response.json() == {"status": "ready", "model_ready": True}


@pytest.mark.skipif(not DEFAULT_MODEL_PATH.is_file(), reason="Model artifact is not present")
def test_predict_returns_reference_quality(
    example_wine: dict[str, float | str], monkeypatch: pytest.MonkeyPatch
) -> None:
    monkeypatch.delenv("WINE_MODEL_PATH", raising=False)

    with TestClient(app) as client:
        response = client.post("/predict", json=example_wine)

    assert response.status_code == 200
    # Reference prediction for the shipped example input and current trusted artifact.
    assert response.json()["predicted_quality"] == pytest.approx(5.1675, abs=1e-8)


@pytest.mark.skipif(not DEFAULT_MODEL_PATH.is_file(), reason="Model artifact is not present")
def test_predict_rejects_invalid_request(
    example_wine: dict[str, float | str], monkeypatch: pytest.MonkeyPatch
) -> None:
    monkeypatch.delenv("WINE_MODEL_PATH", raising=False)
    example_wine["color"] = "pink"

    with TestClient(app) as client:
        response = client.post("/predict", json=example_wine)

    assert response.status_code == 422


def test_predict_returns_service_unavailable_without_model(
    example_wine: dict[str, float | str],
    monkeypatch: pytest.MonkeyPatch,
    tmp_path: Path,
) -> None:
    monkeypatch.setenv("WINE_MODEL_PATH", str(tmp_path / "missing.joblib"))

    with TestClient(app) as client:
        health = client.get("/health")
        prediction = client.post("/predict", json=example_wine)

    assert health.json() == {"status": "model_unavailable", "model_ready": False}
    assert prediction.status_code == 503
