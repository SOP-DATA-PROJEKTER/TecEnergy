import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { RoomService } from 'src/app/services/room.service';
import { SimpleInfo } from 'src/app/models/SimpleInfo';
import { MeterData } from 'src/app/models/MeterData';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-room',
  standalone: true,
  imports: [CommonModule,NavbarComponent,SidebarComponent,DashboardComponent],
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit
{
  constructor(private roomService : RoomService, private route : ActivatedRoute, private router: Router) {}

  CurrentRoomId : number = 1;
  Room : MeterData = {Name : "", Current : 0, Accumulated : 0, Note : ""}
  Meters : MeterData[] = [];

  Building : SimpleInfo = {Id : 0, Name : ""}
  RoomList : SimpleInfo[] = [];

  ngOnInit(): void 
  {
    this.route.params.subscribe(params => 
    {
      this.CurrentRoomId = parseFloat(params['id']);
      this.roomService.getMeterData(params['id']).subscribe(x => this.Room = x);
      this.roomService.getSubMeterData(params['id']).subscribe(x => this.Meters = x);
    });

    this.roomService.getParentSimpleInfo().subscribe(x => this.Building = x);
    this.roomService.getSiblingsSimpleInfo().subscribe(x => this.RoomList = x);
  }

  SideBarClick(id:number)
  {
    this.router.navigate(['room', id]);
  }
}
