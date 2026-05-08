import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { Navigation } from "../Component/navigation/navigation";

@Component({
  selector: 'app-root',
  standalone: true, // חייב אם זה קומפוננטה עצמאית
  imports: [RouterOutlet, HttpClientModule, Navigation],
  templateUrl: './app.html',
  styleUrls: ['./app.scss'] // שימי לב: styleUrls (במילה) ולא styleUrl
})
export class App {
  protected readonly title = signal('Angular');
}
