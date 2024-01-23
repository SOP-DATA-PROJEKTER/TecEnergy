import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = 
[  
  {
    path: '',
    loadComponent: () => import('./components/frontpage/frontpage.component').then(m => m.FrontpageComponent)
  },
  {
    path: 'room/:id',
    loadComponent: () => import('./components/room/room.component').then(m => m.RoomComponent)
  },
  {
    path: 'meterdetail/:id',
    loadComponent: () => import('./components/meterdetailpage/meterdetailpage.component').then(m => m.MeterdetailpageComponent)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
