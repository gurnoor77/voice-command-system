import { useState, useEffect } from 'react'
import axios from 'axios'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid, Cell } from 'recharts'
import * as signalR from '@microsoft/signalr'

const API = 'https://localhost:7123/api'

interface Command {
  id: number
  commandText: string
  recognizedAt: string
}

interface Stat {
  command: string
  total: number
}

const COLORS = ['#3B82F6', '#8B5CF6', '#10B981', '#F59E0B', '#EF4444', '#06B6D4', '#EC4899', '#84CC16']

const getBadgeColor = (cmd: string) => {
  if (cmd.includes('open')) return 'bg-blue-100 text-blue-700'
  if (cmd.includes('show')) return 'bg-green-100 text-green-700'
  if (cmd.includes('lock') || cmd.includes('exit')) return 'bg-red-100 text-red-700'
  if (cmd.includes('window')) return 'bg-purple-100 text-purple-700'
  return 'bg-gray-100 text-gray-700'
}

function App() {
  const [commands, setCommands] = useState<Command[]>([])
  const [stats, setStats] = useState<Stat[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')
  const [isLive, setIsLive] = useState(false)
  const [lastUpdated, setLastUpdated] = useState<string>('')

  const fetchData = async () => {
    try {
      const [cmdRes, statRes] = await Promise.all([
        axios.get(`${API}/Commands`),
        axios.get(`${API}/Commands/stats`)
      ])
      setCommands(cmdRes.data)
      setStats(statRes.data)
      setIsLive(true)
      setLastUpdated(new Date().toLocaleTimeString())
    } catch (err) {
      setIsLive(false)
      console.error('API error:', err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchData()
    const interval = setInterval(fetchData, 10000)
    return () => clearInterval(interval)
  }, [])

useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7123/commandHub', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build()

    connection.on('ReceiveCommand', (commandText: string) => {
      console.log('New command received:', commandText)
      fetchData()
    })

    connection.start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.error('SignalR error:', err))

    return () => {
      connection.stop()
    }
  }, [])

  const deleteCommand = async (id: number) => {
    if (!window.confirm('Delete this command?')) return
    await axios.delete(`${API}/Commands/${id}`)
    fetchData()
  }

  const deleteAll = async () => {
    if (!window.confirm('Delete ALL history?')) return
    await axios.delete(`${API}/Commands`)
    fetchData()
  }

  const filtered = commands.filter(c =>
    c.commandText.toLowerCase().includes(search.toLowerCase())
  )

  return (
    <div className="min-h-screen bg-gray-50">

      {/* Navbar */}
      <nav className="bg-gray-900 text-white px-6 py-4 flex justify-between items-center shadow-lg">
        <div className="flex items-center gap-3">
          <span className="text-2xl">🎙️</span>
          <div>
            <h1 className="text-lg font-bold">Voice Command System</h1>
            <p className="text-xs text-gray-400">Real-time Dashboard</p>
          </div>
        </div>
        <div className="flex items-center gap-4">
          {lastUpdated && (
            <span className="text-xs text-gray-400">Updated: {lastUpdated}</span>
          )}
          <div className={`flex items-center gap-2 px-3 py-1.5 rounded-full text-xs font-medium ${isLive ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400'}`}>
            <span className={`w-2 h-2 rounded-full ${isLive ? 'bg-green-400 animate-pulse' : 'bg-red-400'}`}></span>
            {isLive ? 'API Connected' : 'API Offline'}
          </div>
        </div>
      </nav>

      <div className="p-6">

        {/* Stats Cards */}
        <div className="grid grid-cols-3 gap-4 mb-6">
          <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 flex items-center gap-4">
            <div className="w-12 h-12 bg-blue-100 rounded-xl flex items-center justify-center">
              <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Total Commands</p>
              <p className="text-3xl font-bold text-blue-600">{commands.length}</p>
            </div>
          </div>
          <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 flex items-center gap-4">
            <div className="w-12 h-12 bg-green-100 rounded-xl flex items-center justify-center">
              <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Unique Commands</p>
              <p className="text-3xl font-bold text-green-600">{stats.length}</p>
            </div>
          </div>
          <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 flex items-center gap-4">
            <div className="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center">
              <svg className="w-6 h-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z" />
              </svg>
            </div>
            <div>
              <p className="text-sm text-gray-500">Most Used</p>
              <p className="text-xl font-bold text-purple-600 truncate">
                {stats.length > 0 ? stats[0].command : '-'}
              </p>
            </div>
          </div>
        </div>

        {/* Chart */}
        <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 mb-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-700">Command Usage Analytics</h2>
            <span className="text-xs text-gray-400 bg-gray-100 px-2 py-1 rounded-full">
              {stats.length} unique commands
            </span>
          </div>
          {stats.length > 0 ? (
            <ResponsiveContainer width="100%" height={250}>
              <BarChart data={stats} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
                <XAxis dataKey="command" tick={{ fontSize: 12 }} />
                <YAxis tick={{ fontSize: 12 }} />
                <Tooltip
                  contentStyle={{ borderRadius: '8px', border: 'none', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.1)' }}
                />
                <Bar dataKey="total" radius={[6, 6, 0, 0]}>
                  {stats.map((_, index) => (
                    <Cell key={index} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          ) : (
            <div className="flex flex-col items-center justify-center py-16 text-gray-300">
              <p className="text-gray-400">No data yet — start speaking commands</p>
            </div>
          )}
        </div>

        {/* History Table */}
        <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
          <div className="flex justify-between items-center mb-4">
            <div>
              <h2 className="text-lg font-semibold text-gray-700">Command History</h2>
              <p className="text-xs text-gray-400 mt-0.5">{filtered.length} records</p>
            </div>
            <div className="flex gap-3">
              <input
                type="text"
                placeholder="Search commands..."
                value={search}
                onChange={e => setSearch(e.target.value)}
                className="border border-gray-200 rounded-lg px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-300 w-48"
              />
              <button
                onClick={deleteAll}
                className="bg-red-500 text-white px-4 py-1.5 rounded-lg text-sm hover:bg-red-600 transition font-medium"
              >
                Delete All
              </button>
              <button
                onClick={fetchData}
                className="bg-blue-500 text-white px-4 py-1.5 rounded-lg text-sm hover:bg-blue-600 transition font-medium"
              >
                Refresh
              </button>
            </div>
          </div>

          {loading ? (
            <div className="flex items-center justify-center py-16">
              <div className="w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
            </div>
          ) : (
            <div className="overflow-hidden rounded-xl border border-gray-100">
              <table className="w-full text-sm">
                <thead>
                  <tr className="bg-gray-900 text-white">
                    <th className="px-4 py-3 text-left font-medium">ID</th>
                    <th className="px-4 py-3 text-left font-medium">Voice Command</th>
                    <th className="px-4 py-3 text-left font-medium">Timestamp</th>
                    <th className="px-4 py-3 text-left font-medium">Action</th>
                  </tr>
                </thead>
                <tbody>
                  {filtered.map((cmd, i) => (
                    <tr key={cmd.id} className={`border-t border-gray-50 hover:bg-blue-50 transition ${i % 2 === 0 ? 'bg-white' : 'bg-gray-50/50'}`}>
                      <td className="px-4 py-3 text-gray-400 font-mono text-xs">#{cmd.id}</td>
                      <td className="px-4 py-3">
                        <span className={`px-2.5 py-1 rounded-full text-xs font-medium ${getBadgeColor(cmd.commandText)}`}>
                          {cmd.commandText}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-gray-500 text-xs">
                        {new Date(cmd.recognizedAt).toLocaleString()}
                      </td>
                      <td className="px-4 py-3">
                        <button
                          onClick={() => deleteCommand(cmd.id)}
                          className="text-red-400 hover:text-red-600 text-xs font-medium hover:bg-red-50 px-2 py-1 rounded transition"
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                  {filtered.length === 0 && (
                    <tr>
                      <td colSpan={4} className="text-center text-gray-400 py-16">
                        No commands found
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>

      </div>

      {/* Footer */}
      <footer className="text-center py-6 text-xs text-gray-400 border-t border-gray-100 mt-4">
        © 2026 Gurnoor Singh | Built with C# · ASP.NET Core · React
        <span className="mx-2">·</span>
        <span className="text-green-500">Auto-refreshes every 10s</span>
      </footer>

    </div>
  )
}

export default App