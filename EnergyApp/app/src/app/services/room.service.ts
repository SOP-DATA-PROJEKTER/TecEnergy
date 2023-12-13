import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MeterData } from '../models/MeterData';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  url : string = "http://192.168.21.7:2050/api/Room/"

  constructor(private http: HttpClient) { }

  getMeterData(id : number): Observable<MeterData> 
  {
    return this.http.get<MeterData>(this.url+'EnergyDto/' + id);
    // return this.http.get<MeterData>('http://localhost:3001/rooms/'+id);
  }

  getSubMeterData(id : number): Observable<MeterData[]> 
  {
    return this.http.get<MeterData[]>(this.url+'EnergyMeterListDto/' + id);
    //return this.http.get<MeterData[]>('http://localhost:3001/meters/' + 1);
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
    return this.http.get<SimpleInfo[]>(this.url+'SimpleList/224af2ab-a495-44dc-1fb7-08dbef442e9b');
    //return this.http.get<SimpleInfo[]>('http://localhost:3001/rooms');
  }
}
