import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet],
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    standalone: true
})
export class AppComponent {
  title = 'app';
  // Test : MeterData = {Name: "Lokale e131", Current : 15, Accumulated: 9999, Note: "Test Note"};
  // TestList : MeterData[] = [
  //   {Name: "Måler 1", Current : 2, Accumulated: 9999},
  //   {Name: "Måler 2", Current : 5, Accumulated: 9999, Note: ""},
  //   {Name: "Måler 3", Current : 1, Accumulated: 9999, Note: "Test Note"},
  //   {Name: "Måler 4", Current : 6, Accumulated: 9999, Note: "Test Note"},
  //   {Name: "Måler 5", Current : 9, Accumulated: 9999, Note: "Test Note"},
  // ];
}