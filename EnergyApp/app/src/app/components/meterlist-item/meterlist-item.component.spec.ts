import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeterlistItemComponent } from './meterlist-item.component';

describe('MeterlistItemComponent', () => {
  let component: MeterlistItemComponent;
  let fixture: ComponentFixture<MeterlistItemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MeterlistItemComponent]
    });
    fixture = TestBed.createComponent(MeterlistItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
