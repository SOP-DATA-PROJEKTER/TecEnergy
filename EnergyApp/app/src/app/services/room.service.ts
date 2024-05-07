import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  Observable,
  switchMap,
  timer
} from 'rxjs';
import { RoomData } from '../models/RoomData';
import { SimpleInfo } from '../models/SimpleInfo';

@Injectable({
  providedIn: 'root',
})
export class RoomService {
  // url: string = 'https://localhost:7227/api/Room/';
  // url : string = "http://192.168.21.7:2050/api/Room/"
  url: string = 'http://10.233.134.113/api/Room/';

  constructor(private http: HttpClient) { }

  getRoomDataStream(roomId: string, intervalMs: number): Observable<RoomData> {
    return timer(0, intervalMs).pipe(
      switchMap(() => this.http.get<RoomData>(this.url + 'MeterData/' + roomId))
    );
  }

  getAllRoomsSimpleInfo(): Observable<SimpleInfo[]> {
    return this.http.get<SimpleInfo[]>(this.url + 'SimpleInfo');
  }

  getRoomDataOnce(roomId: string): Observable<RoomData> {
    return this.http.get<RoomData>(this.url + 'MeterData/' + roomId);
  }
}
