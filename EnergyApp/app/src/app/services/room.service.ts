import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MeterData } from '../models/MeterData';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';
import { SimpleRoom } from '../models/SimpleRoom';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  // url : string = "http://10.233.134.112:2050/api/Room/"
  // url : string = "https://localhost:7141/api/"
  url : string = "https://localhost:7141/api/Room/"

  constructor(private http: HttpClient) { }

  getMeterData(id : string): Observable<MeterData> 
  {
    return this.http.get<MeterData>(this.url+'EnergyDto/' + id);
    // return this.http.get<MeterData>(this.url+'EnergyDto/' + '4E10F56A-147E-4541-ADE4-08DBEF4BCA36');
    // return this.http.get<MeterData>(this.url+'EnergyData/' + id);
    // return this.http.get<MeterData>('http://localhost:3001/rooms/'+id);
  }

  getSubMeterData(id : string): Observable<MeterData[]> 
  {
    return this.http.get<MeterData[]>(this.url+'EnergyMeterListDto/' + id);
    // return this.http.get<MeterData[]>(this.url+'EnergyMeterListDto/' + 'DDADA893-0D4C-41E6-F1C7-08DBFA4515B7');
    // return this.http.get<MeterData[]>(this.url+'EnergyMeter/' + id);
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
    // return this.http.get<SimpleInfo[]>(this.url+'SimpleList/E48642F7-4193-4828-9F9F-08DBFBBFA201');
    return this.http.get<SimpleInfo[]>(this.url+'SimpleList/224AF2AB-A495-44DC-1FB7-08DBEF442E9B');
    // return this.http.get<SimpleInfo[]>(this.url+'EnergyMeter/SimpleList/E48642F7-4193-4828-9F9F-08DBFBBFA201');
    //return this.http.get<SimpleInfo[]>('http://localhost:3001/rooms');
  }

  getInitialRoomId(): Observable<SimpleRoom>
  {
    return this.http.get<SimpleRoom>(this.url+'roomId');
  }

}
