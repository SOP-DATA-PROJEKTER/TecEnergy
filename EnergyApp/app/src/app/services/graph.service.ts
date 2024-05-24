import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DateValue } from '../models/DateValue';

@Injectable({
  providedIn: 'root',
})
export class GraphService {
  constructor(private http: HttpClient) {}

  // url : string = "https://localhost:7227/api/Graph" // Localhost
  // url: string = 'http://192.168.21.7:2050/api/Graph'; // SQL Server
  url: string = 'http://10.233.134.113/api/Graph/'; // Energy Awareness Room
  // url: string = 'http://192.168.5.131:5252/api/Graph'; // Niklas PC

  getDailyGraphData(roomId: string, date: Date): Observable<DateValue[]> {
    return this.http.get<DateValue[]>(
      `${this.url}/Daily/${roomId}/${this.dateConverter(date)}`
    );
  }

  getMonthlyGraphData(roomId: string, date: Date): Observable<DateValue[]> {
    return this.http.get<DateValue[]>(
      `${this.url}/Monthly/${roomId}/${this.dateConverter(date)}`
    );
  }

  getYearlyGraphData(roomId: string): Observable<DateValue[]> {
    return this.http.get<DateValue[]>(`${this.url}/Yearly/${roomId}`);
  }

  dateConverter(date: Date): string {
    return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
  }
}
