import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MeterData } from '../models/MeterData';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';
import { SimpleRoom } from '../models/SimpleRoom';
import { YearlyAccumulatedDto } from '../models/YearlyAccumulatedDto';
import { Moment } from 'moment';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  // url : string = "http://10.233.134.112:2050/api/Room/"
  // url : string = "https://localhost:7141/api/"
  url : string = "http://192.168.21.7:2050/api/Room/"

  constructor(private http: HttpClient) { }

  getMeterData(id : string): Observable<MeterData> 
  {
    return this.http.get<MeterData>(this.url+'EnergyDto/' + id);
  }

  getSubMeterData(id : string): Observable<MeterData[]> 
  {
    return this.http.get<MeterData[]>(this.url+'EnergyMeterListDto/' + id);
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
    return this.http.get<SimpleInfo[]>(this.url+'SimpleList/224AF2AB-A495-44DC-1FB7-08DBEF442E9B');
  }

  getInitialRoomId(): Observable<SimpleRoom>
  {
    return this.http.get<SimpleRoom>(this.url+'roomId');
  }


  getRoomYearlyAccumulationList(roomId: string, Year: String): Observable<YearlyAccumulatedDto[]>
  {
    return this.http.get<YearlyAccumulatedDto[]>(`${this.url}YearlyAccumulation/${roomId}/${Year}`)
  }
}
