// src/components/JokerGrid.tsx
import { useEffect, useMemo, useState } from 'react'
import { AgGridReact } from 'ag-grid-react'
import { ModuleRegistry, AllCommunityModule, themeQuartz, colorSchemeDarkBlue, type ColDef } from 'ag-grid-community'
import type { JokerDraw } from '../types/jokerDraw'
import { flattenDraw } from '../utils/flattenJokerDraw'

ModuleRegistry.registerModules([AllCommunityModule])
const darkTheme = themeQuartz.withPart(colorSchemeDarkBlue).withParams({
    accentColor: '#4f79f7',
    borderRadius: 6,
  })

interface JokerGridProps {
    years: string[]
    selectedYear: string
    onYearChange: (year: string) => void
}

export default function JokerGrid({ years, selectedYear, onYearChange }: JokerGridProps) {
  const [rowData, setRowData] = useState<Record<string, string>[]>([])
  const [quickFilter, setQuickFilter] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (!selectedYear) return

    setLoading(true)
    fetch(`/api/joker-data/${selectedYear}`)
      .then((res) => res.json())
      .then((draws: JokerDraw[]) => setRowData(draws.map(flattenDraw)))
      .catch((err) => console.error('Failed to load data:', err))
      .finally(() => setLoading(false))
  }, [selectedYear])

  const columnDefs: ColDef[] = useMemo(
    () => [
      { field: 'drawNumber', headerName: 'Draw #', filter: true, sortable: true },
      { field: 'date', headerName: 'Date', filter: true, sortable: true },
      { field: 'numbers', headerName: 'Numbers', filter: true },
      { field: 'jokerNumber', headerName: 'Joker', filter: true, width: 90},
      { field: 'winnings_5 + 1', headerName: '5+1 Winnings', filter: true },
      { field: 'matches_5 + 1', headerName: '5+1 Matches', filter: true, width: 120 },
      //{ field: 'matches_5', headerName: '5 Matches', filter: true },
      //{ field: 'winnings_5', headerName: '5 Winnings', filter: true },
      // add more prize tier columns as needed
    ],
    []
  )

  return (
    <div>
      <div style={{ display: 'flex', gap: 12, marginBottom: 8 }}>
        <select value={selectedYear} onChange={(e) => onYearChange(e.target.value)}>
          {years.map((y) => (
            <option key={y} value={y}>{y}</option>
          ))}
        </select>

        <input
          type="text"
          placeholder="Search..."
          value={quickFilter}
          onChange={(e) => setQuickFilter(e.target.value)}
          style={{ padding: 6, width: 250 }}
        />
      </div>

      <div style={{ height: 600, width: '100%' }}>
        <AgGridReact
          theme={darkTheme}
          rowData={rowData}
          columnDefs={columnDefs}
          quickFilterText={quickFilter}
          pagination
          paginationPageSize={20}
          loading={loading}
        />
      </div>
    </div>
  )
}