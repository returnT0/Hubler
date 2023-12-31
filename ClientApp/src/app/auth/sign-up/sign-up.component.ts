import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, NgForm, Validators,} from '@angular/forms';
import {Router} from '@angular/router';
import {AuthenticationService, RegistrationData} from '../../service/auth-service/authentication.service';
import {SupermarketService} from '../../service/store-service/store.service';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {
  signUpForm: FormGroup;
  supermarketTitles: string[] = [];
  errorMessage: string = '';
  isDropdownVisible = false;
  selectedMarketTitle: string | null = null;
  showError: boolean = false;


  constructor(
    private authService: AuthenticationService,
    private storeService: SupermarketService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.signUpForm = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      supermarketTitle: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.authService.getSupermarketTitles().subscribe(
        titles => {
          this.supermarketTitles = titles;
          console.log('Supermarket Titles:', this.supermarketTitles);
        },
        error => {
          this.errorMessage = 'Failed to load supermarket titles';
          console.error(error);
        }
    );
  }

  toggleDropdown() {
    this.isDropdownVisible = !this.isDropdownVisible;
  }

  updateSelection(title: string) {
    this.selectedMarketTitle = title;
    this.signUpForm.get('supermarketTitle')?.setValue(title);
    this.toggleDropdown();
  }

  register() {
    if (this.signUpForm.valid) {
      const registrationData: RegistrationData = {
        email: this.signUpForm.value.email,
        password: this.signUpForm.value.password,
        firstName: this.signUpForm.value.firstName,
        lastName: this.signUpForm.value.lastName,
        supermarketTitle: this.signUpForm.value.supermarketTitle
      };

      this.authService.register(registrationData).subscribe({
        next: (response) => {
          console.log('Registration successful', response);
          this.router.navigate(['/sign-in']);
        },
        error: (error) => {
          // Assume error response is in the format { message: "Error message here" }
          this.errorMessage = error.error.message;
          this.showError = true;
          console.error('Registration failed:', error);
        }
      });
    } else {
      this.signUpForm.markAllAsTouched();
    }
  }

  cancel(): void {
    this.showError = false
  }
}
