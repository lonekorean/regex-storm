var elementInput;

$(document).ready(function(){
    // find and save element input element
    elementInput = $('.element_input');

    // search input cue text
    var searchCue = "start typing here...";
    if(elementInput.val() == '')
    {
        elementInput.val(searchCue);
        elementInput.addClass('element_input_cue');
    }

    // store a hashtable of massaged element strings to search, keyed by reference row ID
    var elementsHash = new Array();
    $('.ref_element').each(function(){
        var element = $(this).html();
        element = element.replace(/<em>.*?<\/em>/gi, '');
        element = element.replace('&lt;', '<');
        element = element.replace('&gt;', '>');
        element = element.replace('&amp;', '&');
        elementsHash[$(this).parent().attr('id')] = element;
    });
    
    // clear cue text on search input focus
    elementInput.focus(function(e){
        if(elementInput.val() == searchCue)
        {
            elementInput.val('');
            elementInput.removeClass('element_input_cue');
        }
    });
    
    // reset cue text on search input blur
    elementInput.blur(function(e){
        if(elementInput.val() == '')
        {
            elementInput.val(searchCue);
            elementInput.addClass('element_input_cue');
        }
    });
    
    // search when the search input changes
    elementInput.keyup(function(e){
        // get current search string
        var searchString = elementInput.val();
        
        // initialize array of IDs of tables that should be shown
        var tableIdsToShow =  new Array();
        
        // loop through hashtable
        for(var rowId in elementsHash)
        {
            var row = $('#' + rowId);
            if(elementsHash[rowId].indexOf(searchString) > -1)
            {
                // element matches search string, show row
                row.show();
                
                // remember that the containing table needs to be visible, too
                var tableId = row.parent().parent().attr('id');
                if(jQuery.inArray(tableId, tableIdsToShow) == -1)
                {
                    tableIdsToShow.push(tableId);
                }
            }
            else
            {
                row.hide();
            }
        }
        
        // set visibility for each table based on search results
        $('.ref_table').each(function(){
            if(jQuery.inArray($(this).attr('id'), tableIdsToShow) > -1)
            {
                $(this).show();
            }
            else
            {
                $(this).hide();
            }
        });
        
        // set visibility of "no results" message
        if(tableIdsToShow.length > 0)
        {
            $('.no_results').hide();
        }
        else
        {
            $('.no_results').show();
        }
    });
    
    // remove cue text before submitting form
    $('form').submit(function(){
        if(elementInput.val() == searchCue)
        {
            elementInput.val('');
        }
    });
});
