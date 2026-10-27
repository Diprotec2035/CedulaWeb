<%@ Page Title="" Language="C#" MasterPageFile="~/aplicacion/Site.master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="cedulaweb.diprotec.cl.aplicacion._default" Async="true" %>
<%@ Import Namespace="Modelo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style>
		.noticias-scroll {
			height: 430px;
			overflow-x: hidden;
			overflow-y: auto;
			scrollbar-color: var(--primary-bg-color, #6259ca) #e9ecef;
			scrollbar-width: auto;
		}

		.noticias-scroll::-webkit-scrollbar {
			width: 13px;
		}

		.noticias-scroll::-webkit-scrollbar-track {
			background-color: #e9ecef;
			border: 1px solid #d8dce3;
			border-radius: 10px;
		}

		.noticias-scroll::-webkit-scrollbar-thumb {
			background-color: var(--primary-bg-color, #6259ca);
			border: 3px solid #e9ecef;
			border-radius: 10px;
			min-height: 55px;
		}

		.noticias-scroll::-webkit-scrollbar-thumb:hover {
			background-color: #4e46a8;
		}
	</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<!-- Main Content-->
	<div class="main-content side-content pt-0">
		<div class="side-app">
			<div class="main-container container-fluid">
				<div class="container-about">

					<!-- ROW-1 OPEN -->
					<div class="row">
						<div class="col-lg-12">
							<div class="row py-5">
								<div class="text-center">
									<p>
										Panel informativo general para usuarios de la plataforma.<br />
										Resumen diario con indicadores, avisos y accesos rápidos del sistema.
									</p>
								</div>
							</div>
						</div>
					</div>
					<!-- ROW-1 CLOSED -->

					<!-- ROW-2 OPEN -->
					<div class="row row-sm">
						<div class="col-sm-12 col-md-6 col-lg-3">
							<div class="card custom-card">
								<div class="card-body">
									<div class="d-flex align-items-center">
										<div>
											<p class="mb-1 text-muted">UF</p>

											<h4 class="mb-0 fw-semibold">
												<asp:Literal ID="litUf" runat="server" Text="$ --"></asp:Literal>
											</h4>

											<span class="tx-12 text-muted">
												<asp:Literal ID="litUfFecha" runat="server"></asp:Literal>
											</span>
										</div>

										<div class="ms-auto">
											<span class="avatar avatar-md rounded-circle bg-primary-transparent text-primary">
												<i class="bi bi-currency-dollar fs-20"></i>
											</span>
										</div>
									</div>
								</div>
							</div>
						</div>

						<div class="col-sm-12 col-md-6 col-lg-3">
							<div class="card custom-card">
								<div class="card-body">
									<div class="d-flex align-items-center">
										<div>
											<p class="mb-1 text-muted">Dólar observado</p>

											<h4 class="mb-0 fw-semibold">
												<asp:Literal ID="litDolar" runat="server" Text="$ --"></asp:Literal>
											</h4>

											<span class="tx-12 text-muted">
												<asp:Literal ID="litDolarFecha" runat="server"></asp:Literal>
											</span>
										</div>

										<div class="ms-auto">
											<span class="avatar avatar-md rounded-circle bg-success-transparent text-success">
												<i class="bi bi-currency-dollar fs-20"></i>
											</span>
										</div>
									</div>
								</div>
							</div>
						</div>

						<div class="col-sm-12 col-md-6 col-lg-3">
							<div class="card custom-card">
								<div class="card-body">
									<div class="d-flex align-items-center">
										<div>
											<p class="mb-1 text-muted">Euro</p>

											<h4 class="mb-0 fw-semibold">
												<asp:Literal ID="litEuro" runat="server" Text="$ --"></asp:Literal>
											</h4>

											<span class="tx-12 text-muted">
												<asp:Literal ID="litEuroFecha" runat="server"></asp:Literal>
											</span>
										</div>

										<div class="ms-auto">
											<span class="avatar avatar-md rounded-circle bg-info-transparent text-info">
												<i class="bi bi-bank fs-20"></i>
											</span>
										</div>
									</div>
								</div>
							</div>
						</div>

						<div class="col-sm-12 col-md-6 col-lg-3">
							<div class="card custom-card">
								<div class="card-body">
									<div class="d-flex align-items-center">
										<div>
											<p class="mb-1 text-muted">UTM</p>

											<h4 class="mb-0 fw-semibold">
												<asp:Literal ID="litUtm" runat="server" Text="$ --"></asp:Literal>
											</h4>

											<span class="tx-12 text-muted">
												<asp:Literal ID="litUtmFecha" runat="server"></asp:Literal>
											</span>
										</div>

										<div class="ms-auto">
											<span class="avatar avatar-md rounded-circle bg-warning-transparent text-warning">
												<i class="bi bi-calendar3 fs-20"></i>
											</span>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<!-- ROW-2 CLOSED -->

					<!-- ROW-3 OPEN -->
					<div class="row row-sm">
						<div class="col-xl-8 col-lg-8 col-md-12">
							<div class="card custom-card">
								<div class="card-header border-bottom">
									<div>
										<h5 class="card-title mb-1">Acontecer relevante</h5>

										<p class="tx-13 text-muted mb-0">
											Resumen simple de información general.
										</p>
									</div>
								</div>

								<div class="card-body py-4 ps-4 pe-3">
									<div class="noticias-scroll">
										<asp:Repeater ID="rptNoticias" runat="server">
											<HeaderTemplate>
												<div class="list-group">
											</HeaderTemplate>

											<ItemTemplate>
												<a href='<%# Eval("Url") %>'
													class="list-group-item list-group-item-action flex-column align-items-start"
													target="_blank"
													rel="noopener noreferrer">

													<div class="row g-2 align-items-start">
														<div class="col">
															<h6 class="mb-1">
																<%# Eval("Titulo") %>
															</h6>
														</div>

														<div class="col-auto">
															<small class="text-muted text-end text-nowrap d-block">
																<%# Eval("Fuente") %>
															</small>
														</div>
													</div>

													<p class="mb-1 text-muted">
														<%# Eval("Resumen") %>
													</p>

													<small class="text-muted">
														<%# Eval("FechaPublicacion", "{0:dd-MM-yyyy HH:mm}") %>
													</small>
												</a>
											</ItemTemplate>

											<FooterTemplate>
												</div>
											</FooterTemplate>
										</asp:Repeater>

										<asp:Panel ID="pnlNoticiasNoDisponibles" runat="server" Visible="false">
											<div class="alert alert-light mb-0" role="alert">
												No fue posible obtener noticias relevantes en este momento.
											</div>
										</asp:Panel>
									</div>
								</div>
							</div>
						</div>

						<div class="col-xl-4 col-lg-4 col-md-12">
							<div class="card custom-card">
								<div class="card-header border-bottom">
									<div>
										<h5 class="card-title mb-1">Accesos rápidos</h5>

										<p class="tx-13 text-muted mb-0">
											Opciones frecuentes del sistema.
										</p>
									</div>
								</div>

								<div class="card-body px-4">
									<div class="d-grid gap-1">
										<a href="<%=GetUrlAccesoRapido("usr-a83kd9")%>" class="btn btn-outline-primary text-start py-2 px-3">
											<i class="bi bi-people me-2"></i>
											Usuarios
										</a>

										<a href="<%=GetUrlAccesoRapido("dis-k7m4vz")%>" class="btn btn-outline-primary text-start py-2 px-3">
											<i class="bi bi-phone me-2"></i>
											Dispositivos
										</a>

										<a href="<%=GetUrlAccesoRapido("rul-p6z8mt")%>" class="btn btn-outline-primary text-start py-2 px-3">
											<i class="bi bi-sliders me-2"></i>
											Reglas
										</a>

										<a href="<%=GetUrlAccesoRapido("usr-e45j8i")%>" class="btn btn-outline-primary text-start py-2 px-3">
											<i class="bi bi-bar-chart-steps me-2"></i>
											Visitas
										</a>

										<a href="<%=GetUrlAccesoRapido("vcg-k4t8pz")%>" class="btn btn-outline-primary text-start py-2 px-3">
											<i class="bi bi-upload me-2"></i>
											Cargar visitas
										</a>

										<a href="<%=GetUrlAccesoRapido("vis-rep-c2-8")%>" class="btn btn-outline-primary text-start py-2 px-3">
											<i class="bi bi-clock-history me-2"></i>
											Registro visitas
										</a>
									</div>
								</div>
							</div>

							<div id="divAvisoPlataforma" runat="server" class="card custom-card">
								<div class="card-body">
									<div class="d-flex align-items-start">
										<div class="me-3">
											<span class="avatar avatar-md rounded-circle bg-primary-transparent text-primary">
												<i class="bi bi-info-circle fs-20"></i>
											</span>
										</div>

										<div>
											<h6 class="mb-1">
												<asp:Literal ID="litAvisoTitulo" runat="server"></asp:Literal>
											</h6>

											<p class="tx-13 text-muted mb-1">
												<asp:Literal ID="litAvisoMensaje" runat="server"></asp:Literal>
											</p>

											<p id="pAvisoVersion" runat="server" class="tx-12 text-muted mb-0">
												Versión:
												<asp:Literal ID="litAvisoVersion" runat="server"></asp:Literal>
											</p>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<!-- ROW-3 CLOSED -->

				</div>
			</div>
		</div>
	</div>
	<!-- End Main Content-->
</asp:Content>