import {Component} from '@angular/core';
import {AuthenticationService} from "../authentication.service";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  public sidebarLinks = [
    {route: '/chart', label: 'Home', imgURL: '/assets/assets/home.svg'},
    {route: '/user', label: 'User', imgURL: '/assets/assets/user.svg'},
    {route: '/card', label: 'Card', imgURL: '/assets/assets/store.svg'},
    {route: '/employees', label: 'Employees', imgURL: '/assets/assets/members.svg'},

  ];

  constructor(private authService: AuthenticationService) {
  }

  get isSignedIn(): boolean {
    return this.authService.isUserSignedIn();
  }

  signOut(): void {
    this.authService.signOut();
  }

  public isActive(linkRoute: string, currentPath: string): boolean {
    return (currentPath.includes(linkRoute) && linkRoute.length > 1) || currentPath === linkRoute;
  }
}
