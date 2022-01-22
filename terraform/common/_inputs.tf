
variable "object_prefix" {
  type = string
  validation {
    condition = contains(["world.dev.", "world.prod."], var.object_prefix)
    error_message = "The object prefix must be set to `world.dev.` or `world.prod.`."
  }
}