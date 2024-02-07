import { Injectable } from '@angular/core';
import { BehaviorSubject, interval, Observable, switchMap } from 'rxjs';
import { MeterData } from '../models/MeterData';
import { SimpleInfo } from '../models/SimpleInfo';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RoomService {

  // url : string = "http://192.168.21.7:2050/api/Room/"
  url : string = "https://localhost:7285/api/SimpleInfo/"
  private roomId = 0;

  //Observables
  MeterData : Observable<MeterData>;
  ParentSimpleInfo : Observable<SimpleInfo>;
  SiblingSimpleInfo : Observable<SimpleInfo[]>;
  SubMeterData : Observable<MeterData[]>; 
  
  //BehavierSubjects
  private meterDataSubject : BehaviorSubject<MeterData> = new BehaviorSubject<MeterData>({Id : 0, Name : "", RealTime : 0, Accumulated : 0, Note : ""});
  private parentSimpleInfoSubject : BehaviorSubject<SimpleInfo> = new BehaviorSubject<SimpleInfo>({Id : 0, Name : ""});
  private siblingSimpleInfoSubject : BehaviorSubject<SimpleInfo[]> = new BehaviorSubject<SimpleInfo[]>([]);
  private subMeterDataSubject : BehaviorSubject<MeterData[]> = new BehaviorSubject<MeterData[]>([]); 
  
  constructor(private http: HttpClient) 
  {
    this.MeterData = this.meterDataSubject.asObservable();
    this.ParentSimpleInfo = this.parentSimpleInfoSubject.asObservable();
    this.SiblingSimpleInfo = this.siblingSimpleInfoSubject.asObservable();
    this.SubMeterData = this.subMeterDataSubject.asObservable();
    

    // //Hacky with static value
    // this.getParentSimpleInfo().subscribe(pData => this.parentSimpleInfoSubject.next(pData));
    // this.getSiblingsSimpleInfo().subscribe(sData => this.siblingSimpleInfoSubject.next(sData));

    // this.UpdateLoop();
  }

  GetRoomData(id : number)
  {
    // this.getMeterData(id)
    //   .subscribe(mData =>{ 
    //     this.roomId = mData.Id;
    //     this.meterDataSubject.next(mData);
    //     this.getSubMeterData(mData.Id).subscribe(smData => this.subMeterDataSubject.next(smData));
        
    //     //Hacky with static value
    //     this.getParentSimpleInfo().subscribe(pData => this.parentSimpleInfoSubject.next(pData));
    //     this.getSiblingsSimpleInfo().subscribe(sData => this.siblingSimpleInfoSubject.next(sData));
    //   })
      this.getSiblingsSimpleInfo().subscribe(sData => this.siblingSimpleInfoSubject.next(sData));
  }

  UpdateLoop()
  {
    interval(3000)
    .pipe(
      switchMap(() => this.getMeterData(this.roomId))
    )
    .subscribe((x) => {
      this.meterDataSubject.next(x);
    });
  }


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
    return this.http.get<SimpleInfo[]>(this.url+'siblings/1/1');
    // return this.http.get<SimpleInfo[]>(this.url+'SimpleList/224af2ab-a495-44dc-1fb7-08dbef442e9b');
    //return this.http.get<SimpleInfo[]>('http://localhost:3001/rooms');

  }
}
