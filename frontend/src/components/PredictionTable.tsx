import type { PredictionHistoryItem } from '../types/wine'

interface PredictionTableProps {
  predictions: PredictionHistoryItem[]
}

export function PredictionTable({ predictions }: PredictionTableProps) {
  if (predictions.length === 0) {
    return (
      <div className="table-empty">
        <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 5.5h16v14H4zM4 10h16M9 5.5v14" /></svg>
        No saved predictions yet. Once prediction history is connected, your results will appear here.
      </div>
    )
  }

  return (
    <table className="prediction-table">
      <thead>
        <tr><th>Created</th><th>Color</th><th>Quality score</th></tr>
      </thead>
      <tbody>
        {predictions.map((prediction) => (
          <tr key={prediction.id}>
            <td>{new Date(prediction.createdAt).toLocaleString()}</td>
            <td>{prediction.color}</td>
            <td>{prediction.predictedQuality.toFixed(2)}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
