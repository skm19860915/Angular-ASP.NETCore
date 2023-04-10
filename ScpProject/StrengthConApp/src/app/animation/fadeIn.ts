import { trigger, transition, animate, style, state } from "@angular/animations";

export const fadeInAnimation = trigger('fadeInAnimation', [
    transition(':enter', [
        style({ opacity: '0' }),
        animate(750),
        style({ opacity: '1' }),
    ]),
    transition(':leave', [
        animate(500, style({ opacity: '0' }))
    ]),
    state('*', style({ opacity: '1' })),
]);
