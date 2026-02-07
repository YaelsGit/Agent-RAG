import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true, // חייב אם זה קומפוננטה עצמאית
  imports: [RouterOutlet, HttpClientModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss'] // שימי לב: styleUrls (במילה) ולא styleUrl
})
export class App {
  protected readonly title = signal('Angular');
}
