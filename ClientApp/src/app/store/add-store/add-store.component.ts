import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {SupermarketService, SupermarketWithAddress} from '../../service/store-service/store.service';
import {AuthenticationService} from "../../service/auth-service/authentication.service";

@Component({
  templateUrl: './add-store.component.html'
})
export class AddStoreComponent implements OnInit {
  storeForm: FormGroup;
  title: string = 'Add';
  storeTitle: string | null = null;
  message: string = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private supermarketService: SupermarketService,
    private authService: AuthenticationService
  ) {
    this.storeForm = this.fb.group({
      supermarketId: [0, Validators.required],
      title: ['', Validators.required],
      phone: ['', Validators.required],
      street: ['', Validators.required],
      house: ['', Validators.required],
      city: ['', Validators.required],
      postalCode: ['', Validators.required],
      country: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.storeTitle = this.route.snapshot.paramMap.get('title');
    if (this.storeTitle) {
      this.title = 'Edit';
      this.supermarketService.getSupermarketByTitle(this.storeTitle).subscribe(
        (data: SupermarketWithAddress) => {
          this.storeForm.patchValue(data);
          this.storeForm.get('id')?.setValue(data.id);
        },
        (error: any) => console.error(error)
      );
    }
  }

  saveStore(): void {
    if (this.storeForm.invalid) {
      return;
    }

    const storeData: SupermarketWithAddress = this.storeForm.value;
    if (this.title === 'Add') {
      this.supermarketService.insertSupermarket(storeData).subscribe(
        () => {
          this.message = 'Store been added successfully! Please, wait...';
          setTimeout(() => {
            this.router.navigate(['/stores']);
          }, 1500);
        },
        (error: any) => {
          setTimeout(() => {
            this.message = 'Failed to add!';
          }, 1500);
          console.error(error)
        }
      );
    } else {
      this.supermarketService.updateSupermarket(storeData).subscribe(
        () => {
          this.message = 'Store been updated successfully! Please, wait...';
          setTimeout(() => {
            this.router.navigate(['/stores']);
          }, 1500);
        },
        (error: any) => {
          setTimeout(() => {
            this.message = 'Failed to update!';
          }, 1500);
          console.error(error)
        }
      );
    }
  }

  cancel(): void {
    this.router.navigate(['/stores']);
  }

  public isUserAuthenticated(): boolean {
    return this.authService.isUserSignedIn();
  }
}
