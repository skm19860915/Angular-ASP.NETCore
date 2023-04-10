import { Athlete } from "../../Models/Athlete";

/**
 * Base class for all Weight Room Views.
 * This implements the common function between all of them.
 * @method setSelectedAthlete(athlete: Athlete)
 * @method isSelectedAthlete(athlete: Athlete)
 */
export class WeightRoomBase {
    /**
     * The current selected Athlete
     * Initialized as brand new athlete class.
     */
    selectedAthlete: Athlete = new Athlete();

    /**
     * Mutates selectedAthlete to new given athlete.
     * @param athlete the new athlete to set selectedAthlete to
     */
    setSelectedAthlete(athlete: Athlete) {
        this.selectedAthlete = athlete;
    }

    /**
     * Predicate function to check if the given athlete is
     * the selected athlete.
     * @param athlete the athlete to check against
     */
    isSelectedAthlete(athlete: Athlete): boolean {
        return this.selectedAthlete.Id === athlete.Id;
    }
}
