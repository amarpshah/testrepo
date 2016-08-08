//@todo: temporary function to temporary route the pages 

var route = function(page) {
	document.location.pathname = document.location.pathname.replace(/\w+\.html/, page);
}

/* generate fill in the blank clickable text */
function generateFillInTheBlank() {
	if ($('.questionType.fillInTheBlank').is('.active')) {
		$('.fillInTheBlankSelect').addClass('active');
		$('.fillInTheBlankSelect .generatedQuestion').text( $('.questionForm.active .question').val() );
	}
}

/* resets the question form inputs without the subject, standard and topic */
function resetQuestion() {
	$('.formSection:not(:first)').find('input, textarea, select').val(function() {
		var inputElement = $(this);
		if ( inputElement.is( "input[type='text']" ) || inputElement.is('textarea') ) {
			return "";
		}else if ( inputElement.is( "input[type='radio']" ) || inputElement.is( "input[type='checkbox']" ) ) {
			inputElement.removeAttr('checked')
			return inputElement.val();
		}else if ( inputElement.is( "select" ) ) {
			return "0";
		}
	})
}

var idxOption = 3;

$(document)
	/*clicking the login button displays the login form
	.on('click', '#login .loginBtn', function() {
		$('.heading.popUp').removeClass('active');
		$('#curtain').addClass('active');
		setTimeout(function() {
			$('#curtain').removeClass('active');
			$('.login-popup.popUp').addClass('active');
		}, 1000)
	})
	/* clicking the login submits the login form
	 * It also validates the basic part
	 */
	.on( 'focus', '#topic, #topicPopup',function() {
		// topic autocomplete
		var availableTags = [
			"Thermodynamics",
			"Optics",
			"Algebra"
			];
		$("#topic, #topicPopup").autocomplete({
			source: availableTags
		});
	} )
	.on('click', '#login .proceed', function(e) {
		e.preventDefault();
		var flag = false;
		$('#login input').val(function() {
			if ($(this).val() == "") {
				$(this).prev('label').addClass('error');
			} else{
				flag = true;
			}
			return $(this).val();
		})
		if ($('.error').length) {
			flag = false;
		};
		if (flag) {
			$('#curtain').addClass('active');
			sessionStorage.setItem('name', $('input.username').val());
			setTimeout(function() {
				route('dashboard.html')
			}, 1000)
		};
	})
	/* clicking the add button submits the select standard form
	 * It also validates the basic part
	 */
	.on('click', '.selectStdForm .proceed', function(e) {
		e.preventDefault();
		var flag = false;
		$('.selectStdForm select').val(function() {
			if ($(this).val().match(/select/i)) {
				$(this).prev('label').addClass('error');
			} else{
				flag = true;
			}
			return $(this).val();
		})
		if ($('.error').length) {
			flag = false;
		};
		if (flag) {
			$('.selectStdForm').removeClass('active');
			$('#curtain').removeClass('overlay');
			////var standard = $('.selectStdForm .selectStd').val();
			////var subject = $('.selectStdForm .selectSubject').val();
			////var topic = $('#topicPopup').val();
			setTimeout(function() {
				$('#curtain').removeClass('active');
				//$('.standardSubjectBanner .standard .standardValue').text(standard);
				////$('.standardSubjectBanner .standard').text($('.selectStdForm .selectStd option:selected').text());
				////$('.standardSubjectBanner .subject').text($('.selectStdForm .selectSubject option:selected').text());
				////$('.standardSubjectBanner .topic').text(topic);
				////$('#questionForm .selectStd').val(standard);
				////$('#questionForm .selectSubject').val(subject);
				////$('#questionForm #topic').val(topic);
			}, 1000)
		};
	})
	/* Entering a key removes the error class
	 * 
	 */
	.on('keyup', '#login input, .stdForm input', function() {
		if ($(this).val() != "") {
			$(this).prev('label').removeClass('error');
		}
	})
	/* Changing select option removes the error class
	 * 
	 */
	.on('change', '.stdForm select', function() {
		if (!$(this).val().match(/select/i)) {
				$(this).prev('label').removeClass('error');
			}
	})
	/*clicking the cancel hides the login form*/
	.on('click', '#login .cancel', function() {
		$('.heading.popUp').addClass('active');
		$('.login-popup.popUp').removeClass('active');
	})
	/*clicking the cancel hides the student form*/
	.on('click', '.studentForm .cancel, .stdForm .cancel', function() {
		$('.studentForm, .stdForm').removeClass('active');
		$('#curtain').removeClass('overlay active');
	})
	/*clicking the cancel hides the question form and shopws question control*/
	.on('click', '#questionForm .cancel', function() {
		$('.questionForm, .questionControl').toggleClass('active');
	})
	/*clicking the addStudent shows the student form*/
	.on('click', '.addStudent', function() {
		$('#curtain').addClass('active');
		$('.studentForm form')[0].reset()
		setTimeout(function() {
			$('#curtain').addClass('overlay');
			$('.studentForm').addClass('active');
		}, 1000)
	})
	/*@todo: temporary retrieve username from session storage if present */
	.ready(function() {
		if (sessionStorage.getItem('name') && $('.name').length) {
			$('.name').html(sessionStorage.getItem('name'));
		};
	})
	/*@todo: temporary logout */
	.on('click', '.logout', function() {
		sessionStorage.clear();
		//route('login.html');
	})
	/*.on('click', '.navbar-toggle', function() {
		$('#sidebar').is('.in'){
			$("#page-wrapper").removeClass('active');
		}else{
			$("#page-wrapper").addClass('active');
		}
	})*/
	.on('click', '.standardSubjectBanner', function() {
		$('#curtain').addClass('active');
		setTimeout(function() {
			$('#curtain').addClass('overlay');
			$('.selectStdForm').addClass('active');
		}, 1000)
	})
	/* 
	 * @todo refactor the validation 
	 * clicking the add question
	 * It also validates the basic part
	 */
	.on('click', '.addQuestion', function(e) {
		e.preventDefault();
		//var flag = false;
		//if ($('.questionControl select').val() == "0") {
		//	alert("Please select question type!");
		//} else{
		//	flag = true;
		//}
		//if (flag) {
		//	$('#curtain').addClass('active');
		//	$('.easy, .medium, .difficult').removeClass('active');
		//	$('.questionForm, .questionControl').toggleClass('active');
		//	setTimeout(function() {
		//		$('#curtain').addClass('overlay');
		//		$('.questionForm').addClass('active');
		//		$('.questionForm .questionType').removeClass('active');
		//		$('.questionForm .questionType[data-type="' + $('.questionControl select').val() + '"]').addClass('active');
		//		$('.questionForm .type').val( $('.questionControl select').val() );
		//		$('#curtain').removeClass('overlay active');
		//	}, 1000);
		//};
	})
	/*  removes the active state on click of any button in add question popup */
	.on('click', '.questionForm .next-step .cancel', function() {
		$('.questionForm').removeClass('active');
	})
	/* clicking the add button submits the question form
	 * @todo Do the basic validation part
	 */
	.on('click', '.questionForm .proceed', function(e) {
		e.preventDefault();
		var flag = true;
		var index = $('.questionList .questionItem').length + 1;
		var superscript = 'th';
		var standard = $('.selectStd').val();
		var date = new Date();
		var questionType = $('#questionForm .type').find('option:selected').data('type');
		var isUpdate = $(this).is('.update') ? true : false;
		var isMatchTheFollowing = $('.questionType.active').is('.matchTheFollowing') ? true : false;
		var isSingleChoice = $('.questionType.active').is('.singleChoice') ? true : false;
		var isMultiChoice = $('.questionType.active').is('.multiChoice') ? true : false;
		var isDescriptive = $('.questionType.active').is('.descriptive') ? true : false;
		var isTrueOrFalse = $('.questionType.active').is('.trueOrFalse') ? true : false;
		

		if ( standard== "1" ) {
			superscript = 'st'
		}else if ( standard = "2") {
			superscript = 'nd'
		}else if ( standard = "3") {
			superscript = 'rd'
		}
		/*$('.selectStdForm select').val(function() {
			if ($(this).val().match(/select/i)) {
				$(this).prev('label').addClass('error');
			} else{
				flag = true;
			}
			return $(this).val();
		})
		if ($('.error').length) {
			flag = false;
		};*/
		/* @todo refactor and validate */
		var questionText = '';
		if ($('.generatedQuestion').text().length) {
			questionText = $('.generatedQuestion').html();
		}else {
			questionText = $('textarea.question').val();
		}
		if (flag) {
			setTimeout(function() {
				var questionObject = $('\
					<div class="questionItem">\
                        <span class="questionCheck">\
                            <input type="checkbox" id="checkbox' + index + '">\
                        </span>\
                        <span class="questionTypeIcon">' + questionType + '</span>\
                        <span class="questionDetail" \
							data-standard ="' + $(".selectStd").val() + '"\
							data-subject ="' + $(".selectSubject").val() + '"\
							data-topic ="' + $("#topic").val() + '"\
							data-code ="' + $(".code").val() + '"\
							data-objective ="' + $(".objective").val() + '"\
							data-hint ="' + $(".hint").val() + '"\
							data-questiontype ="' + $(".type").val() + '"\
							data-points ="' + $(".points").val() + '"\
							data-status ="' + $(".status").val() + '"\
							data-difficulty ="' + $(".difficulty").val() + '"\
							data-question ="' + $("textarea.question").val() + '"\
							>\
                            <label for="checkbox' + index + '" href="" class="list-group-item question">\
                                <h5>\
                                    <span class="questionCode">' + $(".code").val() + '</span>: \
                                    <span class="questionText">' + questionText + '</span>\
                                </h5>\
                                <p class="topicText">' + $('#topic').val() + '</p>\
                                <p class="standardInfo">\
                                    <span class="standardText">' + $('.selectStd').val() + '<sup>' + superscript + '</sup> - </span>\
                                    <span class="subjectText">' + $('#questionForm .selectSubject option:selected').text() + '</span>\
                                </p>\
                                <p class="questionState">\
                                    <span class="statusText">' + $('.status option:selected').text() + '</span> - <span class="difficultyText">' + $('.difficulty option:selected').text() + '</span>\
                                </p>\
                            </label>\
                        </span>\
                        <span class="modifiedBy text-right">\
                            <span class="modifiedByName">' + $('.nav .name').text() + '</span> <span class="modifiedDate">' + date.toDateString() + '</span>\
                        </span>\
                        <span class="questionItemAction">\
                            <a href="#" class="btn btn-primary editQuestion pull-right"><i class="fa fa-edit"></i></a>\
                            <a href="#" class="btn btn-warning lockQuestion pull-right"><i class="fa fa-lock"></i></a>\
                            <a href="#" class="btn btn-danger deleteQuestion pull-right"><i class="fa fa-trash"></i></a>\
                        </span>\
                    </div>\
				')
				if (isMatchTheFollowing) {
					var matches     = [];
					$('.optionMatch').each(function(){
						matches.push({
							optionQns: $(this).val(),
							optionAns: $(this).parent().next().find('input').val()
						})
					})
					questionObject.find('.questionDetail').attr( 'data-matches', JSON.stringify(matches) );

				};
				if (isSingleChoice || isMultiChoice) {
					var options     = [];
					$('.questionType.active .optionChoiceAns').each(function(){
						options.push({
							optionChecked: $(this).prop('checked'),
							optionLabel:  $(this).parent().next().val()
						})
					})
					if (isMultiChoice) {
						questionObject.find('.questionDetail').attr( 'data-pointsperchoice', $('.pointsPerChoice').val() );
					}
					questionObject.find('.questionDetail').attr( 'data-options', JSON.stringify(options) );
				}
				if (isDescriptive) {
					questionObject.find('.questionDetail').attr( 'data-keywords', $('.keywords').val() );
				}
				if (isTrueOrFalse) {
					var options     = [];
					questionObject.find('.questionDetail').attr( 'data-displaytype', $('.displayType').val() );
					questionObject.find('.questionDetail').attr( 'data-displaygroup', $('.displayGroup input:checked').attr('class') );
				}

				if (isUpdate) {
					$('.questionForm .proceed span, .questionForm, .questionControl').toggleClass('active');
					$('.updating').html(questionObject[0].innerHTML).removeClass('updating');
					isUpdate = false;
				}else {
					$('.questionList .list-group').append(questionObject);
				}
				resetQuestion();
				$('.fillInTheBlankSelect').removeClass('active');
				$('.displayGroup').removeClass('active');
				$('.difficulty').siblings().removeClass('active');
			}, 1000)
		};
	})
	/* on click of the clickable text*/
	.on('click', '.questionForm .clickableWords', function() {
		$(this).toggleClass('fillBlank');
	})
	/* on click of the search/add toggle buttons*/
	.on('click', '.toggleAdd, .toggleSearch', function() {
		$('.addBlock').toggleClass('active');
	})
	/* on change of the difficulty value shows the label */
	.on('change', '.difficulty', function() {
		var difficulty = parseInt( $(this).val() );
		if (difficulty > 10) {
			difficulty = 10;
			$(this).val('10');
		};
		if (difficulty == 0 || null) {
			$('.easy, .medium, .difficult').removeClass('active');
		}else {
			switch( difficulty ){
				case 1:
				case 2:
				case 3:
					$('.easy, .medium, .difficult').removeClass('active');
					$('.easy').addClass('active');
					break;
				case 4:
				case 5:
				case 6:
					$('.easy, .medium, .difficult').removeClass('active');
					$('.medium').addClass('active');
					break;
				case 7:
				case 8:
				case 9:
				case 10:
					$('.easy, .medium, .difficult').removeClass('active');
					$('.difficult').addClass('active');
					break;
			}
		}
	})
	/* on change of the true or false sets the answer*/
	.on('change', '.displayType', function() {
		$('.answerText').val($(this).find('option:selected').text());
	})
	/* on click of the add more options */
	.on('click', '.addMoreMatcher', function(e) {
		e.preventDefault();
		var numberOfOptions = $('.questionType.active .optionMatch').length + 1;
		$('.questionType.active .form-group-inner').append('\
			<div class="matchWrap">\
				<div class="col-lg-6 col-md-6">\
					<input type="text" class="form-control optionMatch choiceA optionMatch' + numberOfOptions + '">\
				</div>\
		        <div class="col-lg-6 col-md-6">\
		        	<input type="text" class="form-control optionAns choiceB optionAns' + numberOfOptions + '">\
		       		<a href="#" class="removeMatch"><i class="fa fa-fw fa-times-circle"></i></a>\
		       	</div>\
		    </div>\
		')
	})
	/* on click of the remove last added option */
	.on('click', '.removeMatch', function(e) {
		e.preventDefault();
		if ( $('.optionMatch').length > 2 ) {
			var result = confirm("Are you sure you want to delete?");
			if (result) {
				$(this).parent().prev().remove().end().parent().remove();
			}
		}
	})
	/* on click of the remove last added option for choice */
	//.on('click', '.removeOption', function(e) {
	//	e.preventDefault();
	//	if ( $('.questionType.active .optionChoice').length > 2 ) {
	//		var result = confirm("Are you sure you want to delete?");
	//		if (result) {
	//			$(this).closest('.choiceWrapper').remove();
	//		}
	//	}
	//})
    /* adds active class on login */
    .on('click', '.form-signin button', function () {
        $('#curtain').addClass('active')
    })
    /* adds new option for single choice */
    .on('click', '.addMoreOption', function(e) {
		e.preventDefault();
		var type  = "single",
			input = "radio";
		if ($(this).closest('.questionType').is('.multiChoice')) {
			type  = "multi";
			input = "checkbox";
		}
		$('.' + type + 'ChoiceOptions').append('\
			<div class="col-lg-6 col-md-6 col-sm-6 choiceWrapper">\
                <div class="input-group">\
                    <span class="input-group-addon">\
                    	<input type="' + input + '" class=" optionChoiceAns optionChoiceAns' + idxOption + '" name="' + type + 'Choice">\
                    </span>\
			        <input type="text" class="form-control optionChoice optionChoice' + idxOption + '" />\
                    <a href="#" class="removeOption"><i class="fa fa-fw fa-times-circle"></i></a>\
				</div>\
			</div>\
		')
		idxOption++;
	})
	/* on change of the single choice options sets the answer */
	.on('change', '[name="singleChoice"]', function() {
		$('.answerText').val( $('[name="singleChoice"]:checked + input').val() );
	})
	/* on change of the question type changes the section */
	.on('change', '.questionForm .type', function() {
		$('.questionForm .questionType').removeClass('active');
		$('.questionForm .questionType[data-type="' + $('.questionForm .type').val() + '"]').addClass('active');
	})
	/* on change of the multi choice options sets the answer */
	.on('change', '.multiChoice input[type="checkbox"]', function() {
		var answer = '';
		$('.multiChoice input[type="checkbox"]:checked + input').each(function(){
			answer += $(this).val() + ', ';
		})
		$('.answerText').val(answer);
	})
	/* on change of the display type shows the type */
	.on('change', '.displayType', function() {
		var display = $(this),
			name    = display.find('option:selected').data('name'),
			value1  = display.find('option:selected').data('value1'),
			value2  = display.find('option:selected').data('value2');
		if (display.val() != 0) {
			$('.displayGroup').addClass('active');
			$('.displayGroup').find('.correct').attr({'id': value1, 'name': name, 'value': value1});
			$('.displayGroup').find('.correct + label').attr('for', value1).text(value1);
			$('.displayGroup').find('.wrong').attr({'id': value2, 'name': name, 'value': value2});
			$('.displayGroup').find('.wrong + label').attr('for', value2).text(value2);
		}else {
			$('.displayGroup').removeClass('active');
		}
	})
	/* on blur generates the spans of the fill in the blank */
	.on('blur', '.form-control.question', function() {
		if ($('.questionType.fillInTheBlank').is('.active')) {
			$('.fillInTheBlankSelect .generatedQuestion').html('');
			var words = $('.questionForm.active .question').val().split(/\s/);
			for (var i = 0; i <= words.length - 1; i++) {
				var wordspan = $('<span class="clickableWords"></span>');
				wordspan.html(words[i]);
				$('.fillInTheBlankSelect .generatedQuestion').append(wordspan);
			};
		}
	})
	/* on click of the edit button */
	.on('click', '.editQuestion', function(e) {
		e.preventDefault();
		$('.questionForm .proceed').addClass('update');
		$('.questionForm .proceed span').toggleClass('active');
		$(this).closest('.questionItem').addClass('updating')
		$('.questionControl').removeClass('active');
		$('.questionForm').addClass('active');
		var info = $(this).parent().siblings('.questionDetail');
		var form = $('.questionForm');
		var isDescriptive = info.data('questiontype') == "1" ? true : false;
		var isTrueOrFalse = info.data('questiontype') == "2" ? true : false;
		var isMatchTheFollowing = info.data('questiontype') == "3" ? true : false;
		var isSingleChoice = info.data('questiontype') == "4" ? true : false;
		var isMultiChoice = info.data('questiontype') == "5" ? true : false;
		var isFillInTheBlanks = info.data('questiontype') == "6" ? true : false;
		
		form.find('.selectStd').val( info.data('standard') );
		form.find('.selectSubject').val( info.data('subject') );
		form.find('#topic').val( info.data('topic') );
		form.find('.code').val( info.data('code') );
		form.find('textarea.question').val( info.data('question') );
		form.find('.objective').val( info.data('objective') );
		form.find('.hint').val( info.data('hint') );
		form.find('.type').val( info.data('questiontype') );
		form.find('.points').val( info.data('points') );
		form.find('.status').val( info.data('status') );
		form.find('.difficulty').val( info.data('difficulty') );
		form.find('.points').val( info.data('points') );
		form.find('.type, .status, .difficulty').trigger('change');

		if (isDescriptive) {
			form.find('.keywords').val( info.data('keywords') )
		}else if (isTrueOrFalse) {
			form.find('.displayType')
				.val( info.data('displaytype') )
				.trigger('change');
			form.find('.displayGroup ' + '.' + info.data('displaygroup'))[0].checked = true;
		}else if (isMatchTheFollowing) {
			var matches = info.data('matches');
			for (var i = 0; i < matches.length; i++) {
				if ($('.matchWrap')[i] == undefined) {
					$('.addMoreMatcher').trigger('click');
				}else {
					$('.optionMatch')[i].value = matches[i].optionQns;
					$('.optionAns')[i].value = matches[i].optionAns;
				}
			}
		}else if (isSingleChoice || isMultiChoice) {
			var options = info.data('options');
			for (var i = 0; i < options.length; i++) {
				if ($('.choiceWrapper')[i] == undefined) {
					$('.addMoreOption').trigger('click');
				}else {
					if (options[i].optionChecked) {
						$('.questionType.active .optionChoiceAns')[i].checked = true;
					}
					$('.questionType.active .optionChoice')[i].value = options[i].optionLabel;
				}
			}
			if (isMultiChoice) {
				form.find('.pointsPerChoice').val( info.data('pointsperchoice') );
			}
		}else if (isFillInTheBlanks) {
			$('.generatedQuestion').html( $('span.questionText').children() );
			$('.fillInTheBlankSelect').addClass('active');
		}
	})
	/* on click of the lock button */
	.on('click', '.lockQuestion', function(e) {
		e.preventDefault();
		$('#curtain').addClass('active');
		$(this).siblings('.editQuestion').trigger('click');
		$('.status').val("2");
		$('.questionForm .proceed').trigger('click');
		$('#curtain').removeClass('active');
	})
	/* on click of the remove button */
	.on('click', '.deleteQuestion', function(e) {
		e.preventDefault();
		var okDelete = confirm("Are you sure?")
		if (okDelete) {
			$(this).closest('.questionItem').remove();
		}
	})