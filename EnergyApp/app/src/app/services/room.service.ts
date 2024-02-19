import { Injectable } from '@angular/core';
import { interval, map, Observable, of, switchMap } from 'rxjs';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';
import { RoomData } from '../models/RoomData';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  url : string = "http://192.168.21.7:2050/api/Room/"

  constructor(private http: HttpClient) { }

  getRoomDataStream(roomId : string, intervalMs : number) : Observable<RoomData>
  {
    return interval(intervalMs).pipe(
      switchMap(() => this.http.get<RoomData>(this.url +"MeterData/" + roomId))
    );
  }

  getAllRoomsSimpleInfo() : Observable<SimpleInfo[]> 
  {
    return this.http.get<SimpleInfo[]>(this.url + "SimpleInfo");
  }

  getRoomDataOnce(roomId : string) : Observable<RoomData>
  {
    return this.http.get<RoomData>(this.url + "MeterData/" + roomId);
  }


}
