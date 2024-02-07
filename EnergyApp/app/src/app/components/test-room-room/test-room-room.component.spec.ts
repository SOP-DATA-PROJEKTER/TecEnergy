import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestRoomRoomComponent } from './test-room-room.component';

describe('TestRoomRoomComponent', () => {
  let component: TestRoomRoomComponent;
  let fixture: ComponentFixture<TestRoomRoomComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TestRoomRoomComponent]
    });
    fixture = TestBed.createComponent(TestRoomRoomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
