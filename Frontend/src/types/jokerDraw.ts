export interface PrizeTier {
  category: string
  matches: string
  winnings: string
}

export interface JokerDraw {
  drawNumber: string
  date: string
  numbers: number[]
  jokerNumber: number
  totalColumns: string
  prizes: PrizeTier[]
}