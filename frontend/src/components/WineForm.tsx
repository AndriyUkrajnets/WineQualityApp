import { useState, type FormEvent } from 'react'
import type { WineInput } from '../types/wine'

type NumericFeature = Exclude<keyof WineInput, 'color'>
type WineDraft = Record<NumericFeature, string> & { color: WineInput['color'] }

const featureFields: Array<{ key: NumericFeature; label: string; unit?: string }> = [
  { key: 'fixed_acidity', label: 'Fixed acidity', unit: 'g/dm³' },
  { key: 'volatile_acidity', label: 'Volatile acidity', unit: 'g/dm³' },
  { key: 'citric_acid', label: 'Citric acid', unit: 'g/dm³' },
  { key: 'residual_sugar', label: 'Residual sugar', unit: 'g/dm³' },
  { key: 'chlorides', label: 'Chlorides', unit: 'g/dm³' },
  { key: 'free_sulfur_dioxide', label: 'Free sulfur dioxide', unit: 'mg/dm³' },
  { key: 'total_sulfur_dioxide', label: 'Total sulfur dioxide', unit: 'mg/dm³' },
  { key: 'density', label: 'Density', unit: 'g/cm³' },
  { key: 'pH', label: 'pH' },
  { key: 'sulphates', label: 'Sulphates', unit: 'g/dm³' },
  { key: 'alcohol', label: 'Alcohol', unit: '% vol' },
]

const initialDraft: WineDraft = {
  fixed_acidity: '7.4',
  volatile_acidity: '0.7',
  citric_acid: '0.0',
  residual_sugar: '1.9',
  chlorides: '0.076',
  free_sulfur_dioxide: '11',
  total_sulfur_dioxide: '34',
  density: '0.9978',
  pH: '3.51',
  sulphates: '0.56',
  alcohol: '9.4',
  color: 'red',
}

interface WineFormProps {
  isSubmitting: boolean
  onSubmit: (wine: WineInput) => void
}

export function WineForm({ isSubmitting, onSubmit }: WineFormProps) {
  const [draft, setDraft] = useState<WineDraft>(initialDraft)
  const [formError, setFormError] = useState<string | null>(null)

  function updateFeature(key: NumericFeature, value: string) {
    setDraft((current) => ({ ...current, [key]: value }))
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const numericEntries = featureFields.map(({ key }) => [key, Number(draft[key])] as const)
    if (numericEntries.some(([, value]) => !Number.isFinite(value))) {
      setFormError('Enter a valid number for each wine measurement.')
      return
    }

    setFormError(null)
    onSubmit(Object.fromEntries([...numericEntries, ['color', draft.color]]) as WineInput)
  }

  return (
    <form className="panel form-panel" onSubmit={handleSubmit}>
      <div className="panel-header">
        <div className="panel-title">
          <span className="section-icon" aria-hidden="true">
            <svg viewBox="0 0 24 24"><path d="M5 20h14M7 20v-3.5l2.2-3.2V4h5.6v9.3l2.2 3.2V20M9.2 9h5.6" /></svg>
          </span>
          <div>
            <h2>Wine measurements</h2>
            <p>Enter the laboratory characteristics</p>
          </div>
        </div>
        <span className="measure-count">12 INPUTS</span>
      </div>

      <div className="feature-grid">
        {featureFields.map(({ key, label, unit }) => (
          <div className="field" key={key}>
            <label htmlFor={key}>
              {label}{unit && <span className="unit">{unit}</span>}
            </label>
            <input
              id={key}
              name={key}
              type="number"
              step="any"
              required
              value={draft[key]}
              onChange={(event) => updateFeature(key, event.target.value)}
              autoComplete="off"
            />
          </div>
        ))}
      </div>

      {formError && <p className="error-message form-error" role="alert">{formError}</p>}

      <div className="form-bottom">
        <fieldset className="color-picker">
          <legend className="color-label">Wine color</legend>
          <div className="color-option">
            <input
              id="color-red"
              type="radio"
              name="color"
              value="red"
              checked={draft.color === 'red'}
              onChange={() => setDraft((current) => ({ ...current, color: 'red' }))}
            />
            <label htmlFor="color-red"><span className="color-swatch red" />Red</label>
          </div>
          <div className="color-option">
            <input
              id="color-white"
              type="radio"
              name="color"
              value="white"
              checked={draft.color === 'white'}
              onChange={() => setDraft((current) => ({ ...current, color: 'white' }))}
            />
            <label htmlFor="color-white"><span className="color-swatch white" />White</label>
          </div>
        </fieldset>

        <button className="submit-button" type="submit" disabled={isSubmitting}>
          {isSubmitting ? (
            <><span className="spinner" aria-hidden="true" />Calculating</>
          ) : (
            <>
              Predict quality
              <svg viewBox="0 0 20 20" aria-hidden="true"><path d="M3.5 10h12M10.5 5l5 5-5 5" /></svg>
            </>
          )}
        </button>
      </div>
    </form>
  )
}
