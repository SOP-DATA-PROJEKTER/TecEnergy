import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MeterData } from '../models/MeterData';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  constructor(private http: HttpClient) { }

  getMeterData(id : number): Observable<MeterData> 
  {
    return this.http.get<MeterData>('http://localhost:3001/rooms/'+id);
  }

  getSubMeterData(id : number): Observable<MeterData[]> 
  {
    return this.http.get<MeterData[]>('http://localhost:3001/meters/'+id);
  }

  getParentSimpleInfo(): Observable<SimpleInfo> 
  {
    return new Observable(o => 
    {
      o.next({Id:1, Name:"E"}); 
     });
  }

  getSiblingsSimpleInfo(): Observable<SimpleInfo[]> 
  {
    return this.http.get<SimpleInfo[]>('http://localhost:3001/rooms');
  }
}
