import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { SIGNALR_BASE_URL } from './config';
import { getToken } from './authService';

class SignalRService {
  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl(SIGNALR_BASE_URL, {
        accessTokenFactory: () => getToken() 
      }) 
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();
  }

  async startConnection() {
    try {
      // Проверяем, что текущее состояние Disconnected
      if (this.connection.state === 'Disconnected') {
        await this.connection.start();
        console.log('SignalR соединение установлено');
      } else {
        console.log(`Невозможно запустить соединение, текущее состояние: ${this.connection.state}`);
      }
    } catch (error) {
      console.error('Ошибка установки SignalR соединения:', error);
    }
  }

  on(eventName, callback) {
    this.connection.on(eventName, callback);
  }

  off(eventName, callback) {
    this.connection.off(eventName, callback);
  }

  async joinRoom(roomId) {
    const numericRoomId = parseInt(roomId, 10);
    if (this.connection.state !== 'Connected') {
      await this.startConnection();
    }

    try {
      // Ожидаем, что сервер вернет объект: { joinGame, playerOne, playerTwo }
      const result = await this.connection.invoke('JoinRoom', numericRoomId);
      return result;
    } catch (error) {
      console.error('Ошибка при вызове JoinRoom:', error);
      throw error;
    }
  }

  async joinGame(roomId) {
    const numericRoomId = parseInt(roomId, 10);
    if (this.connection.state !== 'Connected') {
      await this.startConnection();
    }

    try {
      // Ожидается объект: { isSuccess, playerNumber, errorMessage }
      const result = await this.connection.invoke('JoinGame', numericRoomId);
      return result;
    } catch (error) {
      console.error('Ошибка при вызове JoinGame:', error);
      throw error;
    }
  }

  async makeMove(moveValue) {
    if (this.connection.state !== 'Connected') {
      await this.startConnection();
    }

    try {
      await this.connection.invoke('MakeMove', moveValue);
    } catch (error) {
      console.error('Ошибка при вызове MakeMove:', error);
      throw error;
    }
  }

  async leaveGame() {
    const token = getToken();
    if (this.connection.state !== 'Connected') {
      await this.startConnection();
    }

    try {
      await this.connection.invoke('LeaveGame', token);
    } catch (error) {
      console.error('Ошибка при вызове LeaveGame:', error);
      throw error;
    }
  }

  async leaveRoom() {
    const token = getToken();
    if (this.connection.state !== 'Connected') {
      await this.startConnection();
    }

    try {
      await this.connection.invoke('LeaveRoom', token);
    } catch (error) {
      console.error('Ошибка при вызове LeaveRoom:', error);
      throw error;
    }
  }
}

export default SignalRService;