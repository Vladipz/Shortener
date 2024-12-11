import { FormControl } from '@angular/forms';

export interface LoginFormData {
  email: FormControl<string>;
  password: FormControl<string>;
}
