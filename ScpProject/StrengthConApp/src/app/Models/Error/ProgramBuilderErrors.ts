export class ValidationError extends Error {
    constructor(m: string) {
        super(m);

        Object.setPrototypeOf(this, ValidationError.prototype);
    }
    public DayNumber: number;
    public ExerciseType : string;
    public ValidationErrorMessage : string;

    public SetError(_dayNumber: number, _exerciseType: string){
        this.DayNumber = _dayNumber;
        this.ExerciseType= _exerciseType;
    }
}

export class ValidationErrorContainer extends Error{
    constructor(m: string) {
        super(m);

        Object.setPrototypeOf(this, ValidationError.prototype);
    }

    public AllValidationErrors : ValidationError[] =[];

}