import { Pipe, PipeTransform } from '@angular/core';
import { DropDownListItem } from './Models/DropDownListItem';
import { ITaggable } from './Interfaces/ITaggable';
import { TagModel } from './Models/TagModel';
import { DomSanitizer } from '@angular/platform-browser';
import { pipe } from 'rxjs';

@Pipe({
    name: 'DropDownListItemFilterPipe'
})
export class DropDownListItemFilterPipe implements PipeTransform {
    transform(value: DropDownListItem[], input: string) {
        if (input) {
            input = input.toLowerCase();
            return value.filter(item => item.Name.indexOf(input) !== -1)
        }
        return value;
    }
}
@Pipe({ name: 'SearchThroughTags' })
export class SearchThroughTagsPipe implements PipeTransform {
    transform(listToFilter: TagModel[], searchString: string): TagModel[] {
        let ret = [];

        if (listToFilter != undefined && searchString != undefined && searchString.length > 0) {
            listToFilter.forEach(item => {
                if (item.display == null || item.display.toLowerCase().includes(searchString.toLowerCase())) {
                    ret.push(item);
                }
            });
        }
        return ret;
    }
}


@Pipe({
    name: 'TagFilterPipe'
})
export class TagFilterPipe implements PipeTransform {
    transform(listToFilter: ITaggable[], filters: TagModel[]): any[] {
        if (listToFilter != undefined && filters != undefined && filters.length > 0) {
            var ret = [];
            listToFilter.forEach(item => {
                var tagsFound = 0;
                for (var i = 0; i <= filters.length - 1; i++) {
                    if (item.Tags.find(target => { return target.Name.toLowerCase() == filters[i].display.toLowerCase() }) != undefined)
                        tagsFound++;
                }
                if (tagsFound == filters.length) { ret.push(item) };
            });
            return ret;

        }
        else { return listToFilter; }

    }
}
@Pipe({
    name: 'ExcludeTagFilterPipe'
})
export class ExcludeTagFilterPipe implements PipeTransform {
    transform(listToFilter: ITaggable[], filters: TagModel[]) {

        if (listToFilter != undefined && filters != undefined && filters.length > 0) {
            var ret = [];
            listToFilter.forEach(item => {
                var tagFound = false;
                for (var i = 0; i <= filters.length - 1; i++) {
                    if (item.Tags.find(target => { return target.Name.toLowerCase() == filters[i].display.toLowerCase() }) != undefined) {
                        tagFound = true;
                        break;
                    }
                }
                if (!tagFound) { ret.push(item) };
            });
            return ret;
        }
        else { return listToFilter; }

    }
}
@Pipe({
    name: 'SearchTaggableFilterPipe'
})
export class SearchTaggableFilterPipe implements PipeTransform {
    transform(listToFilter: ITaggable[], searchString: string) {

        if (listToFilter != undefined && searchString != undefined && searchString.length > 0) {
            var ret = [];
            listToFilter.forEach(item => {
                if (item.Name == null || item.Name.toLowerCase().includes(searchString.toLowerCase())) {
                    ret.push(item);
                }
            });
            return ret;
        }
        else { return listToFilter; }
    }
}

@Pipe({
    name: "Sort"
})
export class ArraySortPipe implements PipeTransform {
    transform(array: any, field: string): any[] {
        if (!Array.isArray(array)) {
            return;
        }
        array.sort((a: any, b: any) => {
            if (a[field] < b[field]) {
                return -1;
            } else if (a[field] > b[field]) {
                return 1;
            } else {
                return 0;
            }
        });
        return array;
    }
}

@Pipe({
    name: "HideDeletedSortPipe"
})
export class HideDeletedSortPipe implements PipeTransform {
    transform(array: any, ShowHiddenInformation: boolean): any[] {
        if (ShowHiddenInformation) {
            return array.filter(item => item.IsDeleted === 1 || item.IsDeleted === true);
        }
        else {
            if (!Array.isArray(array)) {
                return array;
            }
            return array.filter(item => item.IsDeleted === 0 || item.IsDeleted === false);
        }
    }
}


@Pipe({
    name: 'encodeUri'
})
export class EncodeUriPipe implements PipeTransform {

    transform(value: any, args?: any): any {
        return encodeURI(value);
    }
}



@Pipe({ name: 'safe' })
export class SafePipe implements PipeTransform {
    constructor(private sanitizer: DomSanitizer) { }
    transform(url) {
        return this.sanitizer.bypassSecurityTrustResourceUrl(url);
    }
}

@Pipe({
    name: 'BasicStringSearch',
    pure: false
})
export class BasicStringSearchPipe implements PipeTransform {
    transform(items: string, filter: string): any {
        if (!items || !filter) {
            return items;
        }
        var ret: string[];
        for (var i = 0; i < items.length; i++) {
            if (items[i].toLowerCase().includes(filter)) {
                ret.push();
            }
        }
        // filter items array, items which match and return true will be
        // kept, false will be filtered out
        return ret;
    }
}
