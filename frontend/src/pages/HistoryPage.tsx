import { PredictionTable } from '../components/PredictionTable'

export function HistoryPage() {
  return (
    <>
      <div className="page-heading">
        <div>
          <p className="eyebrow">YOUR PREDICTIONS</p>
          <h1>A record of every<br />tasting profile.</h1>
          <p className="subtitle">Review previous predictions and compare wine measurements over time.</p>
        </div>
        <div className="heading-note">HISTORY · COMING WITH DATABASE SUPPORT</div>
      </div>

      <section className="panel history-panel">
        <div className="history-note">
          <svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="9" /><path d="M12 10.5v5M12 7.5h.01" /></svg>
          <div>
            <strong>Prediction history is not connected yet</strong>
            <p>The .NET API currently returns predictions without saving them. History will be enabled when SQL Server storage and the history endpoint are added.</p>
          </div>
        </div>
        <PredictionTable predictions={[]} />
      </section>
    </>
  )
}
