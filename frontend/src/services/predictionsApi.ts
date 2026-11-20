import type { ApiHealth, PredictionResult, WineInput } from '../types/wine'

const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '')

async function readError(response: Response): Promise<string> {
  try {
    const problem = (await response.json()) as { detail?: string; title?: string }
    return problem.detail ?? problem.title ?? `Request failed (${response.status})`
  } catch {
    return `Request failed (${response.status})`
  }
}

export async function getApiHealth(signal?: AbortSignal): Promise<ApiHealth> {
  const response = await fetch(`${apiBaseUrl}/health`, { signal })
  if (!response.ok) throw new Error(await readError(response))
  return (await response.json()) as ApiHealth
}

export async function predictWine(wine: WineInput): Promise<PredictionResult> {
  const response = await fetch(`${apiBaseUrl}/api/predictions`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(wine),
  })

  if (!response.ok) throw new Error(await readError(response))
  return (await response.json()) as PredictionResult
}
