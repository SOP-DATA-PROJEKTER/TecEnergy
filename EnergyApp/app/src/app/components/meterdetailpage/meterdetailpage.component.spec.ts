import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeterdetailpageComponent } from './meterdetailpage.component';

describe('MeterdetailpageComponent', () => {
  let component: MeterdetailpageComponent;
  let fixture: ComponentFixture<MeterdetailpageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MeterdetailpageComponent]
    });
    fixture = TestBed.createComponent(MeterdetailpageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
