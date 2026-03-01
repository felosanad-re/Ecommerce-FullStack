import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environment';
import { BehaviorSubject, Subject } from 'rxjs';
import { Notifications } from '../Interfaces/Notifications';
@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  private notificationsSource = new BehaviorSubject<Notifications[]>([]);
  notifications$ = this.notificationsSource.asObservable();

  // Get unread notifications Count
  private unreadNotificationsSource = new BehaviorSubject<number>(0);
  unreadNotifications$ = this.unreadNotificationsSource.asObservable();
  // Start Connection
  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/orderHub`, {
        accessTokenFactory: () => localStorage.getItem('token') || '', // Access Token
      }) // Url Project Api
      .withAutomaticReconnect()
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        // Add Admin Group
        this.hubConnection
          .invoke('JoinAdminGroup')
          .catch((err) => console.log(err));
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  }

  // Add Message Listener
  public addMessageListener = () => {
    this.hubConnection.on('OrderStatusTracking', (data) => {
      const current = this.notificationsSource.value;
      this.notificationsSource.next([data, ...current]); // Send Notification

      this.unreadNotificationsSource.next(
        this.unreadNotificationsSource.value + 1,
      );
    });
  };

  public handleDisconnects = () => {
    this.hubConnection.onclose(() => {
      console.log('Connection lost. Attempting to reconnect...');
      setTimeout(() => this.startConnection(), 3000); // Try reconnecting after 3 seconds
    });
  };
}
