/*@todo: temporary function to temporary route the pages */
var route = function(page) {
	document.location.pathname = document.location.pathname.replace(/\w+\.html/, page);
}

var idxOption = 3;

$(document)
	/*clicking the login button displays the login form*/
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
	/*
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
    */
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
			setTimeout(function() {
				$('#curtain').removeClass('active');
				$('.standardSubjectBanner .standard .standardValue').text($('.selectStd').val());
				$('.standardSubjectBanner .subject+.subText').text($('.selectSubject').val());
			}, 1000)
			setTimeout(function() {
				$('.addedAlert').removeClass('active');
			}, 4000)
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
		$("#page-wrapper").toggleClass('active');
	})*/
	.on('click', '.selectStdAndSubject', function() {
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
		var flag = false;
		if ($('.questionControl select').val() == "0") {
			alert("Please select question type!");
		} else{
			flag = true;
		}
		if (flag) {
			$('#curtain').addClass('active');
			document.getElementById("questionForm").reset();
			$('.easy, .medium, .difficult').removeClass('active');
			$('.questionForm, .questionControl').toggleClass('active');
			setTimeout(function() {
				$('#curtain').addClass('overlay');
				$('.questionForm').addClass('active');
				$('.questionForm .questionType').removeClass('active');
				$('.questionForm .questionType[data-type="' + $('.questionControl select').val() + '"]').addClass('active');
				$('#curtain').removeClass('overlay active');
			}, 1000);
		};
	})
	/*  removes the active state on click of any button in add question popup */
	.on('click', '.questionForm .next-step button', function() {
		$('.questionForm').removeClass('active');
	})
	/* clicking the add button submits the question form
	 * @todo Do the basic validation part
	 */
	.on('click', '.questionForm .proceed', function(e) {
		e.preventDefault();
		var question = $('.form-group.questionType.active');
		var flag = true;
		var isMatchTheFollowing = $('.questionType.active').is('.matchTheFollowing') ? true : false;
		var isSingleChoice = $('.questionType.active').is('.singleChoice') ? true : false;
		var isMultiChoice = $('.questionType.active').is('.multiChoice') ? true : false;
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
			questionText = $('.questionType.active .question').val();
		}
		if (flag) {
			$('.questionControl').addClass('active');
			$('#curtain').removeClass('overlay');
			setTimeout(function() {
				$('#curtain').removeClass('active');
				var questionObject = $('\
					<div class="list-group">\
                        <a href="#" class="btn btn-danger deleteQuestion pull-right"><i class="fa fa-trash"></i></a>\
                        <a href="#" class="list-group-item question" \
							data-code ="' + $(".code").val() + '"\
							data-topicId ="' + $(".topicId").val() + '"\
							data-questionTypeInput ="' + $(".questionTypeInput").val() + '"\
							data-questionTime ="' + $(".questionTime").val() + '"\
							data-points ="' + $(".points").val() + '"\
							data-status ="' + $(".status").val() + '"\
							data-isAnswer ="' + $(".isAnswer").val() + '"\
							data-difficulty ="' + $(".difficulty").val() + '"\
							data-objective ="' + $(".objective").val() + '"\
							data-question ="' + question.find(".question").val() + '"\
							data-hint ="' + question.find(".hint").val() + '"\
                        >\
                            <i class="fa fa-question-circle"></i> <span class="questionText">' + questionText + '</span>\
                        </a>\
                    </div>\
				')
				if (isMatchTheFollowing) {
					var matches     = [],
						matchesList = $('<ul>');
					$('.optionMatch').each(function(){
						var idx = $(this).attr('class').split('optionMatch')[2];
						matchesList.append('<li>' + $('.optionMatch' + idx).val() + '--->' + $('.optionAns' + idx).val() +'</li>');
						matches.push({
							optionQns: $('.optionMatch' + idx).val(),
							optionAns: $('.optionAns' + idx).val(),
							idx:       idx
						})
					})
					questionObject.find('.questionText').append(matchesList);
					questionObject.find('.question').attr( 'data-matches', JSON.stringify(matches) );

				};
				if (isSingleChoice || isMultiChoice) {
					var options     = [],
						optionsList = $('<ul>');
						debugger;
					$('.questionType.active .optionChoiceAns').each(function(){
						var idx = $(this).attr('class').split('optionChoiceAns')[2];
						optionsList.append('<li>' + $('.questionType.active .optionChoice' + idx).val() +'</li>');
						options.push({
							optionChecked: $(this).prop('checked'),
							optionLabels:  $(this).next().val(),
							idx:       idx
						})
					})
					questionObject.find('.questionText').append(optionsList);
					questionObject.find('.question').attr( 'data-options', JSON.stringify(options) );

				};
				$('.questionList').append(questionObject);
			}, 1000)
		};
	})
	/* generate fill in the blank clickable text */
	.on('click', '.createFillInBlank', function() {
		var words = $('.fillInTheBlank .question').val().split(/\s/);
		$('.fillInTheBlankSelect').addClass('active');
		for (var i = 0; i <= words.length - 1; i++) {
			$('.fillInTheBlankSelect .generatedQuestion').append('<span class="clickableWords">'+ words[i] +'</span>');
		};
	})
	/* on click of the clickable text*/
	.on('click', '.clickableWords', function() {
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
	/* on click of the true or false sets the answer*/
	.on('click', '.trueOrFalse.questionType.active .radioGroup button', function(e) {
		e.preventDefault();
		$('.isAnswer').val($(this).text());
	})
	/* on click of the add more options */
	.on('click', '.addMoreMatcher', function(e) {
		e.preventDefault();
		var numberOfOptions = $('.questionType.active .optionMatch').length + 1;
		$('.questionType.active .form-group-inner').append('\
			<div class="col-lg-6 col-md-6">\
				<input type="text" class="form-control optionMatch optionMatch' + numberOfOptions + '">\
			</div>\
	        <div class="col-lg-6 col-md-6">\
	        	<input type="text" class="form-control optionAns optionAns' + numberOfOptions + '">\
	       	</div>\
		')
	})
    /* adds active class on login */
    .on('click', '.form-signin button', function () {
        $('#curtain').addClass('active')
    })
    /* adds new option for single choice */
    .on('click', '.addMoreOption', function(e) {
		e.preventDefault();
		$('.singleChoiceOptions').append('\
			<div class="col-lg-6 col-md-6 col-sm-6 choiceWrapper">\
			    <label class="choiceLabel">\
			        <input type="radio" class=" optionChoiceAns optionChoiceAns' + idxOption + '" name="singleChoice"><input type="text" class="form-control optionChoice optionChoice' + idxOption + '" />\
			    </label>\
			</div>\
		')
		idxOption++;
	})
	/* on change of the single choice options sets the answer */
	.on('change', '[name="singleChoice"]', function() {
		$('.isAnswer').val( $('[name="singleChoice"]:checked + input').val() );
	})
	/* on change of the multi choice options sets the answer */
	.on('change', '.multiChoice input[type="checkbox"]', function() {
		var answer = '';
		$('.multiChoice input[type="checkbox"]:checked + input').each(function(){
			answer += $(this).val() + ', ';
		})
		$('.isAnswer').val(answer);
	})