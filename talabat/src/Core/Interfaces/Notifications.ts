export interface Notifications {
  id: number; // NotificationId
  orderId: number;
  status: string;
  message: string;
  createdAt: string;
  isRead: boolean;
}
