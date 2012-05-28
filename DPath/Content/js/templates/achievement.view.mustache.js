﻿<div class="modal" id="view-goal">
	<div class="modal-header">
		<a class="close" data-dismiss="modal">×</a>
		<h1>{{#m}}{{#Goal}}{{Name}}{{/Goal}}{{/m}}</h1>
	</div>
	<div class="modal-body">
		{{#m}}
		{{#LoggedIn}}
		<!--<div class="row-fluid">-->
			<div class="row-fluid">
				<div class="clearfix" id="oncourse-form">
					<div class="input">
						<textarea id="comment-{{#m}}{{#Goal}}{{Id}}{{/Goal}}{{/m}}" name="Comment" rows="4" maxlength="256" placeholder="This time I..." class="resolution"></textarea>
					</div>
					<div class="pull-right">
						<div class="btn-group">
							<a class="btn btn-primary dropdown-toggle" data-toggle="dropdown" href="#">
								and that's why I am
								<span class="caret"></span>
							</a>
							<ul class="dropdown-menu">
								<li>
									<a href="#" id="add-oncourse-{{#m}}{{#Goal}}{{Id}}{{/Goal}}{{/m}}">on course</a>
								</li>
								<li>
									<a href="#" id="add-astray-{{#m}}{{#Goal}}{{Id}}{{/Goal}}{{/m}}">astray</a>
								</li>
							</ul>
							
						</div>
					</div>
					<div class="pull-right">
						<pre id="counter" class="counter">255</pre>
					</div>
				</div>
			</div>
		{{/LoggedIn}}
		{{/m}}
			<div class="row-fluid">
				<ul class="nav nav-tabs" id="tab-achievements">
					<li class="active">
						<a href="#all" data-toggle="tab">All</a>
					</li>
					{{#m}}
					{{#LoggedIn}}
					<li>
						<a href="#my-resolutions" data-toggle="tab">Mine</a>
					</li>
					{{/LoggedIn}}
					{{/m}}
					<li>
						<a href="#on-course" data-toggle="tab">On course</a>
					</li>
					<li>
						<a href="#astray" data-toggle="tab">Astray</a>
					</li>
				</ul>

				<div class="tab-content">
					<div class="tab-pane active" id="all">
						<div id="all-list">
							{{#m}}
							{{#AllAchievements}}
							<div class="{{CssResolution}} container-fluid">
								<div class="row-fluid">
									<div class="span2">
										<img src="{{GravatarUrl}}"/>
									</div>
									<div class="span10">
										<div class="row">
											<div class="span5">
												{{UserName}}
											</div>
											<div class="span5">
												<span class="pull-right">{{PrettyDate}}</span>
											</div>
										</div>
										<div class="row">
											<div class="span10">
												<p>{{Comment}}</p>
											</div>
										</div>

									</div>
								</div>
							</div>
							{{/AllAchievements}}
							{{/m}}
						</div>
					</div>
					<div class="tab-pane" id="my-resolutions">
						<div id="my-list">
							{{#m}}
							{{#MyAchievements}}
							<div class="{{CssResolution}} container-fluid">
								<div class="row-fluid">
									<div class="span2">
										<img src="{{GravatarUrl}}"/>
									</div>
									<div class="span10">
										<div class="row">
											<div class="span5">
												{{UserName}}
											</div>
											<div class="span5 pull-right">
												<span class="pull-right">{{PrettyDate}}</span>
											</div>
										</div>
										<div class="row">
											<div class="span10">
												<p>{{Comment}}</p>
											</div>
										</div>

									</div>
								</div>
							</div>
							{{/MyAchievements}}
							{{/m}}
						</div>
						</div>
					<div class="tab-pane" id="on-course">
						<div id="on-course-list">
							{{#m}}
							{{#OnCourseAchievements}}
							<div class="{{CssResolution}} container-fluid">
								<div class="row-fluid">
									<div class="span2">
										<img src="{{GravatarUrl}}"/>
									</div>
									<div class="span10">
										<div class="row">
											<div class="span5">
												{{UserName}}
											</div>
											<div class="span5 pull-right">
												<span class="pull-right">{{PrettyDate}}</span>
											</div>
										</div>
										<div class="row">
											<div class="span10">
												<p>{{Comment}}</p>
											</div>
										</div>

									</div>
								</div>
							</div>
							{{/OnCourseAchievements}}
							{{/m}}
						</div>
					</div>
					<div class="tab-pane" id="astray">
						<div id="astray-list">
						{{#m}}
						{{#AstrayAchievements}}
						<div class="{{CssResolution}} container-fluid">
							<div class="row-fluid">
								<div class="span2">
									<img src="{{GravatarUrl}}"/>
								</div>
								<div class="span10">
									<div class="row">
										<div class="span5">
											{{UserName}}
										</div>
										<div class="span5 pull-right">
											<span class="pull-right">{{PrettyDate}}</span>
										</div>
									</div>
									<div class="row">
										<div class="span10">
											<p>{{Comment}}</p>
										</div>
									</div>

								</div>
							</div>
						</div>
						{{/AstrayAchievements}}
						{{/m}}
						</div>
					</div>
				</div>
			</div>
		<!--</div>-->
	</div>
</div>