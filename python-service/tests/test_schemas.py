import pytest
from pydantic import ValidationError

from app.schemas import WineFeatures


def test_accepts_complete_wine(example_wine: dict[str, float | str]) -> None:
    wine = WineFeatures.model_validate(example_wine)

    assert wine.color == "red"
    assert wine.alcohol == 9.4


@pytest.mark.parametrize("color", ["rose", "Red", "", None])
def test_rejects_unsupported_color(
    example_wine: dict[str, float | str], color: str | None
) -> None:
    example_wine["color"] = color

    with pytest.raises(ValidationError):
        WineFeatures.model_validate(example_wine)


def test_rejects_missing_feature(example_wine: dict[str, float | str]) -> None:
    del example_wine["alcohol"]

    with pytest.raises(ValidationError):
        WineFeatures.model_validate(example_wine)


@pytest.mark.parametrize("value", [float("nan"), float("inf"), float("-inf")])
def test_rejects_non_finite_numeric_values(
    example_wine: dict[str, float | str], value: float
) -> None:
    example_wine["density"] = value

    with pytest.raises(ValidationError):
        WineFeatures.model_validate(example_wine)


def test_rejects_unexpected_fields(example_wine: dict[str, float | str]) -> None:
    example_wine["quality"] = 7

    with pytest.raises(ValidationError):
        WineFeatures.model_validate(example_wine)
