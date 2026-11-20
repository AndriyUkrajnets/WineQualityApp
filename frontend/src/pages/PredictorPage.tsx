import { PredictionResult } from '../components/PredictionResult'
import { WineForm } from '../components/WineForm'
import type { PredictionResult as Prediction, WineInput } from '../types/wine'

interface PredictorPageProps {
  prediction: Prediction | null
  error: string | null
  isSubmitting: boolean
  onPredict: (wine: WineInput) => void
}

export function PredictorPage({ prediction, error, isSubmitting, onPredict }: PredictorPageProps) {
  return (
    <>
      <div className="page-heading">
        <div>
          <p className="eyebrow">PHYSICOCHEMICAL PREDICTOR</p>
          <h1>Read the character<br />of your wine.</h1>
          <p className="subtitle">Use laboratory measurements to estimate wine quality with the trained regression model.</p>
        </div>
        <div className="heading-note">
          <svg viewBox="0 0 20 20" aria-hidden="true"><path d="M10 2.5 12 7l4.8.5-3.5 3.3.9 4.7L10 13.2l-4.2 2.3.9-4.7L3.2 7.5 8 7l2-4.5Z" /></svg>
          11 chemistry signals · 1 color profile
        </div>
      </div>

      <div className="predictor-layout">
        <WineForm isSubmitting={isSubmitting} onSubmit={onPredict} />
        <PredictionResult error={error} prediction={prediction} />
      </div>

      <div className="method-strip" aria-label="Model details">
        <div className="method-item">
          <span className="method-icon" aria-hidden="true"><svg viewBox="0 0 20 20"><path d="M3 16.5h14M5 13l3-4 2.2 2 4.8-6" /><path d="M12 5h3v3" /></svg></span>
          <div><strong>Regression model</strong><span>Continuous quality score</span></div>
        </div>
        <div className="method-item">
          <span className="method-icon" aria-hidden="true"><svg viewBox="0 0 20 20"><path d="M10 2.5 16 5v4.3c0 4-2.6 6.6-6 8.2-3.4-1.6-6-4.2-6-8.2V5l6-2.5Z" /><path d="m7.5 9.8 1.7 1.7 3.5-3.8" /></svg></span>
          <div><strong>Training-aligned</strong><span>Preprocessing stays in the pipeline</span></div>
        </div>
        <div className="method-item">
          <span className="method-icon" aria-hidden="true"><svg viewBox="0 0 20 20"><path d="M10 3v14M3 10h14" /><circle cx="10" cy="10" r="7.5" /></svg></span>
          <div><strong>12 inputs</strong><span>Measurements plus wine color</span></div>
        </div>
      </div>
    </>
  )
}
