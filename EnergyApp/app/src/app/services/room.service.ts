import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MeterData } from '../models/MeterData';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  url : string = "http://10.233.134.112:2050/api/Room/"

  constructor(private http: HttpClient) { }

  getMeterData(id : string): Observable<MeterData> 
  {
    return this.http.get<MeterData>(this.url+'EnergyDto/' + id);
    // return this.http.get<MeterData>('http://localhost:3001/rooms/'+id);
  }

  getSubMeterData(id : string): Observable<MeterData[]> 
  {
    return this.http.get<MeterData[]>(this.url+'EnergyMeterListDto/' + id);
    //return this.http.get<MeterData[]>('http://localhost:3001/meters/' + 1);
  }

  getParentSimpleInfo(): Observable<SimpleInfo> 
  {
    return new Observable(o => 
    {
      o.next({Id:"E48642F7-4193-4828-9F9F-08DBFBBFA201", Name:"E"}); 
     });
  }

  getSiblingsSimpleInfo(): Observable<SimpleInfo[]> 
  {
    return this.http.get<SimpleInfo[]>(this.url+'SimpleList/E48642F7-4193-4828-9F9F-08DBFBBFA201');
    //return this.http.get<SimpleInfo[]>('http://localhost:3001/rooms');
  }
}
