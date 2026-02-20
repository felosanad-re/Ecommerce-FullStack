import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DataUserService {
  constructor(private _http: HttpClient) {}
  // عملت بروبيرتي من النوع ده علشان اقدر اجيب اسم اليوزر
  userName: BehaviorSubject<string> = new BehaviorSubject<string>(
    localStorage.getItem('userName') || 'Vistor',
  );
}
