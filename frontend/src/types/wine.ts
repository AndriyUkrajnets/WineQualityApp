export interface WineInput {
  fixed_acidity: number
  volatile_acidity: number
  citric_acid: number
  residual_sugar: number
  chlorides: number
  free_sulfur_dioxide: number
  total_sulfur_dioxide: number
  density: number
  pH: number
  sulphates: number
  alcohol: number
  color: 'red' | 'white'
}

export interface PredictionResult {
  predicted_quality: number
}

export interface ApiHealth {
  status: string
}

export type ApiState = 'checking' | 'online' | 'offline'

export interface PredictionHistoryItem {
  id: number
  createdAt: string
  color: 'red' | 'white'
  predictedQuality: number
}
