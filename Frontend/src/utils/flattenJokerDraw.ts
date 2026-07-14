// src/utils/flattenJokerDraw.ts
import type { JokerDraw } from '../types/jokerDraw'

export function flattenDraw(draw: JokerDraw) {
  const row: Record<string, string> = {
    drawNumber: draw.drawNumber,
    date: draw.date,
    numbers: draw.numbers.join(', '),
    jokerNumber: draw.jokerNumber.toString(),
    totalColumns: draw.totalColumns,
  }

  draw.prizes.forEach((p) => {
    row[`matches_${p.category}`] = p.matches
    row[`winnings_${p.category}`] = p.winnings
  })

  return row
}