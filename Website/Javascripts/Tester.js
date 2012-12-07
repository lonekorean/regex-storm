var prm;                                // instance of page request manager (for .NET async stuff)
var asyncUpdateTimer;                   // timer for async updates (started when user edits form input)
var isAsyncUpdateTimerOn = false;       // flag to denote when the timer is active
var cachedDetailsLinks = new Array();   // contains details links that have already been retrieved
var blockDetailsTabAsync = false;       // used to block duplicate async calls to prevent "tab purgatory"
var enableHaystackBackdrop = true;      // true by default

// commonly referenced elements
var patternInput;
var haystackInput;
var replacementInput;
var startPositionInput;
var matchesLimitInput;

// these values are polled to detect changes to inputs
// (polling sucks, but event triggers alone are not sufficient)
var patternValue;
var haystackValue;
var replacementValue;
var startPositionValue;
var matchesLimitValue;

$(document).ready(function(){
    // get global page request manager
    prm = Sys.WebForms.PageRequestManager.getInstance();

    // wire async event handlers
    prm.add_beginRequest(onBeginRequest);
    prm.add_endRequest(onEndRequest);
    prm.add_pageLoaded(onPageLoaded);
    
    // save commonly referenced elements, so we don't have to repeatedly call
    // DOM-traversing jQuery selectors to get them every time
    patternInput = $('.pattern_input');
    haystackInput = $('.haystack_input');
    replacementInput = $('.replacement_input');
    startPositionInput = $('#' + txtStartPositionId);
    matchesLimitInput = $('#' + txtMatchesLimit);
    
    if($.browser.opera){
        // sorry Opera, the haystack backdrop doesn't work 100% for you
        // I'd rather disable this one feature to give your users a more stable experience
        enableHaystackBackdrop = false;
    }

    if(enableHaystackBackdrop)
    {
        // add the "powered" style to the haystack backdrop div to turn it on
        $('.haystack_backdrop_wrap').addClass('haystack_backdrop_wrap_powered');
        
        // make haystack backdrop scroll with haystack input
        $('.haystack_input').scroll(adjustScroll);

        if($('#' + hfHasMatchesId).val() == "true")
        {
            // make match highlights look pretty
            styleMatchHighlights($('.haystack_backdrop_wrap'));
        };
    }
    
    // remove styles that are used to improve layout when javascript is disabled
    $('.details_content_no_script').removeClass('details_content_no_script');
    $('.details_context_no_script').removeClass('details_context_no_script');

    // hide submit button (async stuff takes care of submitting)
    $('.submit_section').hide();

    // update when any regex option is changed
    // (would use change(), but doesn't work right in IE)
    $('.check_options_block input').click(function(){
        scheduleAsyncUpdate(true);
    });

    // begin polling for changes to inputs
    patternValue = $('.pattern_input').val();
    haystackValue = $('.haystack_input').val();
    replacementValue = $('.replacement_input').val();
    startPositionValue = $('#' + txtStartPositionId).val();
    matchesLimitValue = $('#' + txtMatchesLimit).val();
    setInterval('pollFormChanges()', 50);

    // hide replacement input initially if checkbox not checked
    if(!$('.replacement_toggle_block input').is(':checked'))
    {
        $('.replacement_input_wrap').css('display', 'none');
    }
    
    // toggle replacement input via checkbox
    // (would use change(), but doesn't work right in IE)
    $('.replacement_toggle_block input').click(function(){
        if($('.replacement_toggle_block input').is(':checked'))
        {
            $('.replacement_input_wrap').slideDown(250);
            $('.replacement_input_wrap input').focus();
        }
        else
        {
            $('.replacement_input_wrap').slideUp(250);
        }
        scheduleAsyncUpdate(false);
    });
    
    // show replacement tip
    $('.replacement_tip').show();

    // show tabbed details stuff
    $('.details_tabs').show();
    
    // details info tab is always ready upon page load
    $('#' + hlDetailsInfoId).parent().addClass('details_tab_active');
    cachedDetailsLinks.push(hlDetailsInfoId);

    // hide all other details panels
    $('#' + pnlDetailsTableId).hide();
    $('#' + pnlDetailsContextId).hide();
    $('#' + pnlDetailsSplitId).hide();

    if($('#' + hfHasMatchesId).val() == "true")
    {
        // if the page loads with matches, then it already has all details
        // ready, so we can treat all the other tab links as cached as well
        cachedDetailsLinks.push(hlDetailsTableId);
        cachedDetailsLinks.push(hlDetailsContextId);
        cachedDetailsLinks.push(hlDetailsSplitId);
        
        // also, style context match highlights
        styleMatchHighlights($('.details_context'));
    }
    else
    {
        // page loaded with no matches, so hide the rest of the details tabs
        $('#' + hlDetailsTableId).parent().hide();
        $('#' + hlDetailsContextId).parent().hide();
        $('#' + hlDetailsSplitId).parent().hide();
    };

    // click event for details links to change tabbed view
    $('.details_tabs a').click(function(){
        // prevent action if details tab async is currently blocked
        if(!blockDetailsTabAsync)
        {
            var detailsLinkId = $(this).attr('id');

            if($.inArray(detailsLinkId, cachedDetailsLinks) > -1)
            {
                // this details link has already been loaded, no need for
                // another async call, just show it
                showDetails(detailsLinkId);
            }
            else
            {
                // about to make async call for details tab, begin blocking
                blockDetailsTabAsync = true;
            
                // hide current details panel
                getDetailsPanel(getCurrentDetailsLinkId()).hide();

                // update current details tab to the one that was just clicked
                $('.details_tab_active').removeClass('details_tab_active');
                $(this).parent().addClass('details_tab_active');
                
                // initiate async postback
                doPostBack(false, false, detailsLinkId);
            }
        }
        
        return false;
    });
    
    // setup captures (duh)
    setupCaptures();

    // setup permalink (also duh)
    setupPermalink();
});

function onBeginRequest(sender, e)
{
    // show loading indicator
    $('.details_loading').show();
}

function onEndRequest(sender, e)
{
}

function onPageLoaded(sender, e)
{
    if(prm.get_isInAsyncPostBack())
    {
        if(enableHaystackBackdrop)
        {
            // scroll haystack backdrop to line up with haystack input
            adjustScroll();
        }
        
        // hide loading indicator
        $('.details_loading').hide();

        // find out which details link was updated
        var updatedDetailsLinkId = $('#' + hfDetailsLinkId).val();
        
        // show updated details
        showDetails(updatedDetailsLinkId);

        if($('#' + hfWipeDetailsLinksId).val() == 'true')
        {
            // things have changed, wipe details link cache
            cachedDetailsLinks = [];
        }
        
        if($.inArray(updatedDetailsLinkId, cachedDetailsLinks) == -1)
        {
            // this details link is now cached
            cachedDetailsLinks.push(updatedDetailsLinkId);
        }

        if($('#' + hfHasMatchesId).val() == 'true')
        {
            // matches found, show details tabs
            $('#' + hlDetailsTableId).parent().fadeIn(250);
            $('#' + hlDetailsContextId).parent().fadeIn(250);
            $('#' + hlDetailsSplitId).parent().fadeIn(250);

            if(enableHaystackBackdrop && !isAsyncUpdateTimerOn)
            {
                // redo styles on haystack backdrop matches
                // but only if timer isn't on (prevents lagged highlighting)
                styleMatchHighlights($('.haystack_backdrop_wrap'));
            }

            // redo styles on context match highlights
            styleMatchHighlights($('.details_context'));
        }
        else
        {
            // no matches found, hide details tabs
            $('#' + hlDetailsTableId).parent().fadeOut(250);
            $('#' + hlDetailsContextId).parent().fadeOut(250);
            $('#' + hlDetailsSplitId).parent().fadeOut(250);
        }

        // certain details tabs require additional setup
        switch(updatedDetailsLinkId)
        {
            case hlDetailsInfoId:
                setupPermalink();
                break;
            case hlDetailsTableId:
                setupCaptures();
                break;
            case hlDetailsContextId:
                $('.details_context_no_script').removeClass('details_context_no_script');
                break;
        }
        
        // async call is complete, unblock tabbing
        blockDetailsTabAsync = false;
    }
}

function pollFormChanges()
{
    if(patternInput.val() != patternValue)
    {
        // pattern changed
        scheduleAsyncUpdate(true);
        patternValue = patternInput.val();
    }
    
    if(haystackInput.val() != haystackValue)
    {
        // haystack changed
        scheduleAsyncUpdate(true);
        haystackValue = haystackInput.val();

        // quickly hide highlighting to prevent mismatched highlights
        $('.haystack_backdrop_wrap em').addClass('hide');
    }

    if(replacementInput.val() != replacementValue)
    {
        // replacement changed
        scheduleAsyncUpdate(false);
        replacementValue = replacementInput.val();
    }

    if(startPositionInput.val() != startPositionValue)
    {
        // start position changed
        scheduleAsyncUpdate(true);
        startPositionValue = startPositionInput.val();
    }

    if(matchesLimitInput.val() != matchesLimitValue)
    {
        // matches limit changed
        scheduleAsyncUpdate(true);
        matchesLimitValue = matchesLimitInput.val();
    }
}

function scheduleAsyncUpdate(updateHaystackBackdrop)
{
    // reset timeout (prevents repeated calls while user is still typing)
    clearTimeout(asyncUpdateTimer);

    // set code for timer to call    
    var timerCode = 'doPostBack(' + updateHaystackBackdrop + ', true, getCurrentDetailsLinkId());'
    asyncUpdateTimer = setTimeout(timerCode, 250);

    // we are now waiting to begin async call, set flags as such
    isAsyncUpdateTimerOn = true;
    blockDetailsTabAsync = true;

    // change some visuals to let the user know that stuff is going to happen
    getDetailsPanel(getCurrentDetailsLinkId()).css('opacity', '0.2');
    $('.details_loading').show();
}

function doPostBack(updateHaystackBackdrop, wipeDetailsLinks, updateDetailsLinkId)
{
    // if we're here now, then the timer is done
    isAsyncUpdateTimerOn = false;

    if(!Page_ClientValidate())
    {
        // validation errors already found on client-side, so skip async call
        // and setup the details info tab to show the errors
        $('.regex_info_blurb').hide();
        $('.permalink_wrap').hide();
        showDetails(hlDetailsInfoId);
        
        // hide other details tabs
        $('#' + hlDetailsTableId).parent().fadeOut(250);
        $('#' + hlDetailsContextId).parent().fadeOut(250);
        $('#' + hlDetailsSplitId).parent().fadeOut(250);
    }
    else
    {
        // if there's currently an async call out there, we no longer care about it
        // cancel it before we start this async call
        if(prm.get_isInAsyncPostBack())
        {
            prm.abortPostBack();
        }
        
        if(getDetailsPanel(updateDetailsLinkId).is(':visible'))
        {
            // if the panel to update is visible at all then make sure it is properly faded
            // otherwise, carefully timed form edits can leave the panel at full opacity during async
            getDetailsPanel(updateDetailsLinkId).css('opacity', '0.2');
        }
    
        // updating the haystack backdrop is only relevant if it's been enabled to begin with
        updateHaystackBackdrop = updateHaystackBackdrop && enableHaystackBackdrop;

        // set some outgoing variables
        $('#' + hfUpdateHaystackBackdropId).val(updateHaystackBackdrop);
        $('#' + hfWipeDetailsLinksId).val(wipeDetailsLinks);
        $('#' + hfDetailsLinkId).val(updateDetailsLinkId);

        // initiate async postback
        __doPostBack(btnRunId, '');
    }
}

function styleMatchHighlights(container)
{
    // odd/even selectors in jquery are zero-indexed, which effectively
    // means the classes added here are switched
    container.find('em:odd').addClass('odd');
    container.find('em:even').addClass('even');
}

function showDetails(detailsLinkId)
{
    // change active details tab
    $('.details_tabs a').stop().css('opacity', '');
    $('.details_tab_active').removeClass('details_tab_active');
    $('#' + detailsLinkId).parent().addClass('details_tab_active');

    // change active details panel
    $('.details_section').stop().css('opacity', '');
    $('.details_section:visible').hide();
    getDetailsPanel(detailsLinkId).show();
}

function setupCaptures()
{
    // hide captures initially
    $('.captures').hide();
    $('span.captures_count').each(function(){
        $(this).replaceWith('<a href="#" class="captures_count">' + $(this).text() + '</a>');
    });

    // expand or collapse captures display on click
    $('a.captures_count').click(function(){
        var capturesContainer = $(this).parent().find('.captures');
        
        // toggle bullet image
        var bulletUrlPart = (capturesContainer.is(':visible')) ? 'plus' : 'minus';
        $(this).css('background-image', 'url(Images/Icons/bullet_toggle_' + bulletUrlPart + '.png)');
        
        // toggle display
        capturesContainer.slideToggle(250);
        
        return false;
    });
}

function setupPermalink()
{
    // hide permalink initially
    $('.show_permalink').show();
    $('#' + pnlPermalinkId).hide();

    // click event to reveal permalink
    $('.show_permalink').click(function(){
        $(this).hide();
        $('#' + pnlPermalinkId).fadeIn(250);
        return false;
    });

    // auto select all text when permalink box is clicked
    $('.permalink_input').focus(function(){
        $(this).select();
    });
}

function getCurrentDetailsLinkId()
{
    return $('.details_tab_active a').attr('id');
}

function getDetailsPanel(detailsLinkId)
{
    var panel;
    switch(detailsLinkId)
    {
        case hlDetailsInfoId:
            panel = $('#' + pnlDetailsInfoId);
            break;
        case hlDetailsTableId:
            panel = $('#' + pnlDetailsTableId);
            break;
        case hlDetailsContextId:
            panel = $('#' + pnlDetailsContextId);
            break;
        case hlDetailsSplitId:
            panel = $('#' + pnlDetailsSplitId);
            break;
    }
    return panel;
}

function adjustScroll()
{
    var input = $('.haystack_input');
    var div = $('.haystack_backdrop_wrap div');
    
    div.scrollTop(input.scrollTop());
    div.scrollLeft(input.scrollLeft());
}
