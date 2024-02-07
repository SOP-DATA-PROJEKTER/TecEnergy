import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SimpleInfoTest } from '../models/SimpleInfoTest';

@Injectable({
  providedIn: 'root'
})
export class TestService {

  url : string = "https://localhost:7285/api/SimpleInfo/"
  
  constructor(private http: HttpClient) {}
  
  getSiblingInfo(id : number, infoType : number) : Observable<SimpleInfoTest[]>
  {
    return this.http.get<SimpleInfoTest[]>(this.url +"siblings/" + id + "/" + infoType);
  }

  getMainMeterDataStream()
  {

  }

  getSubMeterDataStream()
  {
    
  }

  // getInfo(id : number) : Observable<SimpleInfo>
  // {
  //   return this.http.get<SimpleInfo>(this.url+id+'/1');
  // }

  // GetInfoOnTimer(delayMS : number) : Observable<SimpleInfo>
  // { 
  //   return interval(delayMS).pipe(
  //     switchMap(() => this.getInfo(1))
  //   );
  // }
}
