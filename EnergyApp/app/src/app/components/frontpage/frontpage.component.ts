import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { RoomService } from 'src/app/services/room.service';

@Component({
  selector: 'app-frontpage',
  standalone: true,
  imports: [CommonModule,NavbarComponent],
  templateUrl: './frontpage.component.html',
  styleUrls: ['./frontpage.component.css']
})
export class FrontpageComponent
{

  constructor(private roomService : RoomService) {}
}
