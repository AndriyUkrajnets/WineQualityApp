import { useEffect, useState } from 'react'
import { HistoryPage } from './pages/HistoryPage'
import { PredictorPage } from './pages/PredictorPage'
import { getApiHealth, predictWine } from './services/predictionsApi'
import type { ApiState, PredictionResult, WineInput } from './types/wine'
import './App.css'

type Page = 'predictor' | 'history'

function App() {
  const [page, setPage] = useState<Page>('predictor')
  const [apiState, setApiState] = useState<ApiState>('checking')
  const [prediction, setPrediction] = useState<PredictionResult | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()

    getApiHealth(controller.signal)
      .then(() => setApiState('online'))
      .catch(() => {
        if (!controller.signal.aborted) setApiState('offline')
      })

    return () => controller.abort()
  }, [])

  async function handlePredict(wine: WineInput) {
    setIsSubmitting(true)
    setError(null)

    try {
      const result = await predictWine(wine)
      setPrediction(result)
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : 'The prediction could not be completed. Please try again.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="app-shell">
      <header className="topbar">
        <a className="brand" href="#predictor" onClick={() => setPage('predictor')}>
          <span className="brand-mark" aria-hidden="true">
            <svg viewBox="0 0 32 32" fill="none">
              <path d="M16 5c-1.7 4.4-5.6 6.9-5.6 12.2A5.6 5.6 0 0 0 16 22.8a5.6 5.6 0 0 0 5.6-5.6C21.6 11.9 17.7 9.4 16 5Z" />
              <path d="M16 22.8v4.1M16 13.2c-3.4 0-5.6-1.5-7.3-4.3M16 16c3.2-.2 5.4-1.7 7.1-4.4" />
            </svg>
          </span>
          <span className="brand-copy">
            <strong>WineQuality</strong>
            <small>MODEL STUDIO</small>
          </span>
        </a>

        <nav className="main-nav" aria-label="Main navigation">
          <button
            className={page === 'predictor' ? 'nav-link active' : 'nav-link'}
            onClick={() => setPage('predictor')}
            type="button"
          >
            Predictor
          </button>
          <button
            className={page === 'history' ? 'nav-link active' : 'nav-link'}
            onClick={() => setPage('history')}
            type="button"
          >
            History <span className="nav-soon">SOON</span>
          </button>
        </nav>

        <div className={`api-status ${apiState}`} role="status" aria-live="polite">
          <span className="status-dot" />
          {apiState === 'checking'
            ? 'Connecting to API'
            : apiState === 'online'
              ? 'API connected'
              : 'API unavailable'}
        </div>
      </header>

      <main className="main-content">
        {page === 'predictor' ? (
          <PredictorPage
            error={error}
            isSubmitting={isSubmitting}
            onPredict={handlePredict}
            prediction={prediction}
          />
        ) : (
          <HistoryPage />
        )}
      </main>

      <footer className="site-footer">
        <span>WINEQUALITY MODEL STUDIO</span>
        <span>Continuous quality estimates from physicochemical data</span>
      </footer>
    </div>
  )
}

export default App
