import {Component} from '@angular/core';
import {NgForm} from '@angular/forms';
import {Router} from '@angular/router';
import {AuthenticationService} from '../../service/auth-service/authentication.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent {
  invalidLogin?: boolean;
  message: boolean = false;

  constructor(private authService: AuthenticationService, private router: Router) {
  }

  public login = (form: NgForm) => {
    this.authService.signIn(form.value).subscribe(
      response => {
        localStorage.setItem('jwt', response.token);
        this.router.navigate(['/user']);
      },
      error => {
        this.message = true;
        setTimeout(() => this.message = false, 1500);
      }
    );
  }

  cancel(): void {
    this.message = false
  }
}
