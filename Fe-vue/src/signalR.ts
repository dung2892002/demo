import * as signalR from '@microsoft/signalr'

const connection: signalR.HubConnection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:7160/api/v1/chatHub', {
    accessTokenFactory: () => localStorage.getItem('accessToken') || '',
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets,
  })
  .withAutomaticReconnect()
  .build()

const startConnection = async () => {
  if (connection.state === signalR.HubConnectionState.Disconnected) {
    try {
      await connection.start()
      console.log('SignalR connected.')
    } catch (err) {
      console.error('SignalR connection failed: ', err)
      setTimeout(startConnection, 1000)
    }
  }
}

connection.onreconnected(() => {
  console.log('Reconnected to SignalR hub.')
})

// Khi mất kết nối, thử kết nối lại
connection.onclose(() => {
  console.log('Disconnected from SignalR hub.')
  startConnection()
})

window.addEventListener('load', () => {
  if (localStorage.getItem('accessToken')) {
    startConnection()
  }
})
connection.serverTimeoutInMilliseconds = 60000
connection.keepAliveIntervalInMilliseconds = 15000

export { connection, startConnection }
