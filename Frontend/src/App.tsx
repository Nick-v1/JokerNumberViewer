import { useEffect, useState } from 'react'
import JokerGrid from './components/JokerGrid'

function App() {
  const [years, setYears] = useState<string[]>([])
  const [selectedYear, setSelectedYear] = useState<string>('')

  useEffect(() => {
    fetch('/api/joker-years')
      .then((res) => res.json())
      .then((data: string[]) => {
        setYears(data)
        if (data.length > 0) setSelectedYear(data[0])
      })
      .catch((err) => console.error('Failed to load years:', err))
  }, [])

  return (
    <div className="app">
      <h1>Joker {selectedYear || '...'} Results</h1>
      <JokerGrid
        years={years}
        selectedYear={selectedYear}
        onYearChange={setSelectedYear}
         />
    </div>
  )
}

export default App
